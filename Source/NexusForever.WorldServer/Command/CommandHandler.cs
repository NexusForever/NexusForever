using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Command.Convert;
using NexusForever.WorldServer.Command.Static;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.RBAC.Static;

namespace NexusForever.WorldServer.Command
{
    public class CommandHandler : ICommandHandler
    {
        public class CommandParameter
        {
            public Type Type { get; }
            public IParameterConvert Converter { get; }
            public bool IsOptional { get; }

            /// <summary>
            /// Create a new <see cref="CommandParameter"/> of the supplied <see cref="System.Type"/> with optional <see cref="IParameterConvert"/> overload and optional flag.
            /// </summary>
            public CommandParameter(Type type, IParameterConvert converter, bool isOptional)
            {
                Type       = type;
                Converter  = converter;
                IsOptional = isOptional;
            }
        }

        private string helpText;
        private Permission permission;
        private Type targetType;

        private MethodContainer methodContainer;
        private ImmutableList<CommandParameter> parameters;

        /// <summary>
        /// Build <see cref="CommandHandler"/> that represents <see cref="MethodInfo"/> on the instance <see cref="CommandCategory"/>.
        /// </summary>
        public void Build(CommandAttribute attribute, CommandCategory category, MethodInfo method)
        {
            permission      = attribute.Permission;
            methodContainer = new MethodContainer(category, method);

            HandleOptionalAttributes(attribute, method);

            ParameterInfo[] parameterInfo = method.GetParameters();
            BuildParameters(attribute, parameterInfo);
            BuildHelp(attribute, parameterInfo);
        }

        private void HandleOptionalAttributes(CommandAttribute attribute, MethodInfo method)
        {
            // CommandTargetAttribute is used to add type constraints to target
            CommandTargetAttribute targetAttribute = method.GetCustomAttribute<CommandTargetAttribute>();
            if (targetAttribute != null)
            {
                if (!typeof(WorldEntity).IsAssignableFrom(targetAttribute.TargetType))
                    throw new CommandException($"Target type from CommandTargetAttribute must be assignable from WorldEntity for CommandHandler {string.Join(',', attribute.Commands)}!");

                targetType = targetAttribute.TargetType;
            }
        }

        private void BuildParameters(CommandAttribute attribute, ParameterInfo[] parameterInfo)
        {
            if (parameterInfo.Length == 0)
                throw new CommandException($"No parameters for CommandHandler {string.Join(',', attribute.Commands)}, ICommandContext must at least be supplied!");

            // first parameter must be the context
            if (!typeof(ICommandContext).IsAssignableFrom(parameterInfo[0].ParameterType))
                throw new CommandException($"First parameter for CommandHandler {string.Join(',', attribute.Commands)} must be ICommandContext!");

            var builder = ImmutableList.CreateBuilder<CommandParameter>();
            foreach (ParameterInfo parameter in parameterInfo.Skip(1))
            {
                Type underlyingType = Nullable.GetUnderlyingType(parameter.ParameterType);

                // if parameter has the ParameterAttribute attribute the converter overload will be used from here
                // otherwise it will come from the default for the parameter type
                ParameterAttribute parameterAttribute = parameter.GetCustomAttribute<ParameterAttribute>();
                IParameterConvert converter = parameterAttribute?.Converter != null
                    ? CommandManager.Instance.GetParameterConverter(parameterAttribute.Converter)
                    : CommandManager.Instance.GetParameterConverterDefault(underlyingType ?? parameter.ParameterType);

                if (converter == null)
                    throw new ArgumentException($"No parameter converter found for parameter {parameter.Name} in CommandHandler {string.Join(',', attribute.Commands)}!");

                // parameter is considered optional if specified explicitly or if is a nullable type
                builder.Add(new CommandParameter(parameter.ParameterType, converter, underlyingType != null || (parameterAttribute?.IsOptional ?? false)));
            }

            parameters = builder.ToImmutable();

            // check that optional parameters are last
            int index = parameters.FindIndex(p => p.IsOptional);
            if (index == -1)
                return;

            for (int i = index; i < parameters.Count; i++)
                if (!parameters[i].IsOptional)
                    throw new CommandException($"Optional parameters must come after mandatory parameters in CommandHandler {string.Join(',', attribute.Commands)}!");
        }

        private void BuildHelp(CommandAttribute attribute, ParameterInfo[] parameterInfo)
        {
            var builder = new StringBuilder();

            // construct help text for command based on CommandAttribute and optional ParameterAttributes
            foreach (string command in attribute.Commands)
            {
                builder.AppendLine($"{command} - {attribute.HelpText}");
                foreach (ParameterInfo parameter in parameterInfo.Skip(1))
                {
                    builder.Append($"Parameter: {parameter.Name} - ");
                    ParameterAttribute parameterAttribute = parameter.GetCustomAttribute<ParameterAttribute>();
                    if (parameterAttribute == null)
                        builder.AppendLine("No help text available");
                    else
                    {
                        if (parameterAttribute.IsOptional)
                            builder.Append("[optional] ");
                        builder.AppendLine($"{parameterAttribute.HelpText}");
                    }
                }
            }

            helpText = builder.ToString();
        }

        /// <summary>
        /// Return help text that provides a brief summery of the <see cref="ICommandHandler"/>.
        /// </summary>
        public string GetHelp(ICommandContext context, bool detailed)
        {
            return helpText;
        }

        /// <summary>
        /// Returns if the supplied <see cref="ICommandContext"/> can invoke <see cref="ICommandHandler"/>.
        /// </summary>
        public CommandResult CanInvoke(ICommandContext context)
        {
            if (permission != Permission.None && !context.Permissions.Contains(permission))
                return CommandResult.NoPermission;
            return CommandResult.Ok;
        }

        /// <summary>
        /// Invoke <see cref="CommandHandler"/>, convert <see cref="ParameterQueue"/> to object parameters before invoking the underlying <see cref="MethodInfo"/>.
        /// </summary>
        public CommandResult Invoke(ICommandContext context, ParameterQueue queue)
        {
            CommandResult result = CanInvoke(context);
            if (result != CommandResult.Ok)
                return result;

            if (targetType != null)
            {
                if (context.Target != null && !targetType.IsInstanceOfType(context.Target))
                    return CommandResult.InvalidTarget;

                // invoker will always exist
                if (!targetType.IsInstanceOfType(context.Invoker))
                    return CommandResult.InvalidTarget;
            }

            try
            {
                // convert ParameterQueue to objects via IParameterConvert interface
                var convertedParameters = new List<object> { context };
                foreach (CommandParameter parameter in parameters)
                {
                    object parameterValue = null;
                    if (queue.Count != 0)
                        parameterValue = parameter.Converter.Convert(context, queue);
                    else
                    {
                        // if we have no commands left in queue and the parameter isn't optional we don't have enough data
                        if (!parameter.IsOptional)
                            return CommandResult.InvalidParameters;

                        // cache this?
                        if (parameter.Type.IsValueType)
                            parameterValue = Activator.CreateInstance(parameter.Type);
                    }

                    convertedParameters.Add(parameterValue);
                }

                methodContainer.Invoke(convertedParameters);
            }
            catch (Exception exception)
            {
                context.SendError(exception.ToString());
            }

            return CommandResult.Ok;
        }

        /// <summary>
        /// Invoke help for <see cref="ICommandHandler"/> with the supplied <see cref="ICommandContext"/> and <see cref="ParameterQueue"/>.
        /// </summary>
        /// <remarks>
        /// This will display help text if the <see cref="ICommandContext"/> has permission to invoke this <see cref="ICommandHandler"/>.
        /// </remarks>
        public CommandResult InvokeHelp(ICommandContext context, ParameterQueue queue)
        {
            CommandResult result = CanInvoke(context);
            if (result != CommandResult.Ok)
                return result;

            context.SendMessage($"Showing help for: {queue.BreadcrumbTrail}");
            context.SendMessage(helpText);
            return CommandResult.Ok;
        }
    }
}
