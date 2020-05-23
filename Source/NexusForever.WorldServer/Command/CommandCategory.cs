using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Text;
using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Command.Static;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.RBAC.Static;

namespace NexusForever.WorldServer.Command
{
    public abstract class CommandCategory : ICommandHandler
    {
        private string helpText;
        private Permission permission;
        private Type targetType;

        private ImmutableDictionary<string, ICommandHandler> handlers;

        public void Build(CommandAttribute attribute)
        {
            helpText   = $"{string.Join(", ", attribute.Commands)} - {attribute.HelpText ?? "No help text available"}";
            permission = attribute.Permission;

            HandleOptionalAttributes(attribute);
            BuildChildren();
        }

        private void HandleOptionalAttributes(CommandAttribute attribute)
        {
            // CommandTargetAttribute is used to add type constraints to target
            CommandTargetAttribute targetAttribute = GetType().GetCustomAttribute<CommandTargetAttribute>();
            if (targetAttribute != null)
            {
                if (!typeof(WorldEntity).IsAssignableFrom(targetAttribute.TargetType))
                    throw new CommandException($"Target type from CommandTargetAttribute must be assignable from WorldEntity for CommandHandler {string.Join(',', attribute.Commands)}!");

                targetType = targetAttribute.TargetType;
            }
        }

        private void BuildChildren()
        {
            var builder = ImmutableDictionary.CreateBuilder<string, ICommandHandler>(
                StringComparer.InvariantCultureIgnoreCase);

            // child handlers
            foreach (MethodInfo method in GetType().GetMethods())
            {
                CommandAttribute attribute = method.GetCustomAttribute<CommandAttribute>();
                if (attribute == null)
                    continue;

                var handler = new CommandHandler();
                handler.Build(attribute, this, method);

                foreach (string command in attribute.Commands)
                    builder.Add(command, handler);
            }

            // child categories
            foreach (Type type in GetType().GetNestedTypes())
            {
                CommandAttribute attribute = type.GetCustomAttribute<CommandAttribute>();
                if (attribute == null)
                    continue;

                CommandCategory category = (CommandCategory)Activator.CreateInstance(type);
                category.Build(attribute);

                foreach (string command in attribute.Commands)
                    builder.Add(command, category);
            }

            handlers = builder.ToImmutable();
        }

        /// <summary>
        /// Return help text that provides a brief summery of the <see cref="ICommandHandler"/>.
        /// </summary>
        public void GetHelp(StringBuilder builder, ICommandContext context, bool detailed)
        {
            builder.AppendLine(helpText);

            if (detailed)
            {
                foreach (ICommandHandler handler in handlers.Values)
                {
                    if (handler.CanInvoke(context) != CommandResult.Ok)
                        continue;

                    builder.Append(handler is CommandCategory ? "Category: " : "Command: ");
                    handler.GetHelp(builder, context, false);
                }
            }
        }

        /// <summary>
        /// Returns if the supplied <see cref="ICommandContext"/> can invoke <see cref="ICommandHandler"/>.
        /// </summary>
        public virtual CommandResult CanInvoke(ICommandContext context)
        {
            if (permission != Permission.None && !context.Permissions.Contains(permission))
                return CommandResult.NoPermission;
            return CommandResult.Ok;
        }

        /// <summary>
        /// Invoke <see cref="CommandCategory"/> with the supplied <see cref="ICommandContext"/> and <see cref="ParameterQueue"/>.
        /// </summary>
        public virtual CommandResult Invoke(ICommandContext context, ParameterQueue queue)
        {
            CommandResult result = CanInvoke(context);
            if (result != CommandResult.Ok)
                return result;

            if (queue.Count == 0)
            {
                // no additional commands are present show help for category
                var builder = new StringBuilder();
                builder.AppendLine($"Showing help for: {queue.BreadcrumbTrail}");
                GetHelp(builder, context, true);

                context.SendMessage(builder.ToString());
                return CommandResult.Ok;
            }

            if (targetType != null)
            {
                if (context.Target != null && !targetType.IsInstanceOfType(context.Target))
                    return CommandResult.InvalidTarget;

                // invoker will always exist
                if (!targetType.IsInstanceOfType(context.Invoker))
                    return CommandResult.InvalidTarget;
            }

            string command = queue.Dequeue();
            if (!handlers.TryGetValue(command, out ICommandHandler handler))
                return CommandResult.NoCommand;

            return handler.Invoke(context, queue);
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

            string command = queue.Front;
            if (command == null
                || !handlers.TryGetValue(command, out ICommandHandler handler))
            {
                // no additional commands are present show help for category
                var builder = new StringBuilder();
                builder.AppendLine("-----------------------------------------------");
                builder.AppendLine($"Showing help for: {queue.BreadcrumbTrail}");
                GetHelp(builder, context, true);

                context.SendMessage(builder.ToString());

                return CommandResult.Ok;
            }

            queue.Dequeue();
            return handler.InvokeHelp(context, queue);
        }
    }
}
