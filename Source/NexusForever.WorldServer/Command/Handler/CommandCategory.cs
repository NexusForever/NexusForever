using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Game.Account;

namespace NexusForever.WorldServer.Command.Handler
{
    public abstract class CommandCategory : NamedCommand
    {
        private readonly ImmutableDictionary<string, SubCommandInstance> subCommands;
        public override string HelpText { get; }

        protected CommandCategory(bool requiresSession, params string[] categoryNames)
            : base(requiresSession, categoryNames)
        {
            string topLevelCommand = "";
            var NameAttributeValue = GetType().GetCustomAttributes(typeof(NameAttribute), true);
            if (NameAttributeValue.Length > 0)
            {
                var nameAttribute = (NameAttribute)NameAttributeValue[0];
                topLevelCommand = nameAttribute.Name.Replace(" ", string.Empty);
            }
            
                var helpBuilder = new StringBuilder();
            helpBuilder.AppendLine("--- sub commands");

            Dictionary<string, SubCommandInstance> commandHandlers =
                new Dictionary<string, SubCommandInstance>(StringComparer.OrdinalIgnoreCase);
            foreach (MethodInfo method in GetType().GetMethods()
                    .Where(i => !i.IsStatic && !i.IsAbstract && i.ReturnType == typeof(Task)))
                // Support for multiple attributes.
            foreach (SubCommandHandlerAttribute attribute in method.GetCustomAttributes<SubCommandHandlerAttribute>())
            {
                if (attribute == null)
                    continue;

                ParameterInfo[] parameterInfo = method.GetParameters();

                #region Debug

                Debug.Assert(parameterInfo.Length == 3);
                Debug.Assert(method.ReturnType == typeof(Task));
                Debug.Assert(typeof(CommandContext) == parameterInfo[0].ParameterType);
                Debug.Assert(typeof(string) == parameterInfo[1].ParameterType);
                Debug.Assert(typeof(string[]) == parameterInfo[2].ParameterType);

                #endregion

                helpBuilder.Append($"   {attribute.Command} - ");
                if (string.IsNullOrWhiteSpace(attribute.HelpText))
                    helpBuilder.AppendLine("No help available.");
                else
                    helpBuilder.AppendLine(attribute.HelpText);

                SubCommandInstance subCommand = new SubCommandInstance((SubCommandHandler)Delegate.CreateDelegate(typeof(SubCommandHandler), this, method), attribute.RequiredPermission);
                commandHandlers.Add(attribute.Command, subCommand);
            }

            subCommands = commandHandlers.ToImmutableDictionary();
            HelpText = helpBuilder.ToString();
        }

        protected sealed override async Task HandleCommandAsync(CommandContext context, string command,
            string[] parameters)
        {
            if (parameters.Length > 0)
            {
                if (!subCommands.TryGetValue(parameters[0], out SubCommandInstance commandCallback))
                {
                    await SendHelpAsync(context);
                    return;
                }

                bool isConsole = context.Session == null;

                if (isConsole || RoleManager.HasPermission(context.Session, commandCallback.RequiredPermission))
                    await (commandCallback.Handler?.Invoke(context, parameters[0], parameters.Skip(1).ToArray()) ??
                        Task.CompletedTask);
                else
                    await context.SendMessageAsync($"Your account status is too low for this subcommand: !{command} {string.Join(' ', parameters)} (Requires permission: {commandCallback.RequiredPermission})");
            }
            else
                await SendHelpAsync(context);

            // TODO
        }

        public delegate Task SubCommandHandler(CommandContext context, string subCommand, string[] parameters);
    }
}
