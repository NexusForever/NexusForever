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
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Command.Handler
{
    public abstract class CommandCategory : NamedCommand
    {
        private readonly ImmutableDictionary<string, SubCommandHandler> subCommands;
        public override string HelpText { get; }

        protected CommandCategory(bool requiresSession, params string[] categoryNames)
            : base(requiresSession, categoryNames)
        {
            var helpBuilder = new StringBuilder();
            helpBuilder.AppendLine("--- sub commands");

            Dictionary<string, SubCommandHandler> commandHandlers =
                new Dictionary<string, SubCommandHandler>(StringComparer.OrdinalIgnoreCase);
            foreach (MethodInfo method in GetType().GetMethods()
                    .Where(i => !i.IsStatic && !i.IsAbstract && i.ReturnType == typeof(Task)))
                // Support for multiple attributes.
            foreach (SubCommandHandlerAttribute attribute in method.GetCustomAttributes<SubCommandHandlerAttribute>())
            {
                if (attribute == null)
                    continue;

                ParameterInfo[] parameterInfo = method.GetParameters();

                #region Debug

                Debug.Assert(parameterInfo.Length == 4);
                Debug.Assert(method.ReturnType == typeof(Task));
                Debug.Assert(typeof(CommandContext) == parameterInfo[0].ParameterType);
                Debug.Assert(typeof(string) == parameterInfo[1].ParameterType);
                Debug.Assert(typeof(string[]) == parameterInfo[2].ParameterType);
                Debug.Assert(typeof(IEnumerable<ChatFormat>) == parameterInfo[3].ParameterType);

                #endregion

                    helpBuilder.Append($"   {attribute.Command} - ");
                if (string.IsNullOrWhiteSpace(attribute.HelpText))
                    helpBuilder.AppendLine("No help available.");
                else
                    helpBuilder.AppendLine(attribute.HelpText);

                commandHandlers.Add(attribute.Command,
                    (SubCommandHandler) Delegate.CreateDelegate(typeof(SubCommandHandler), this, method));
            }

            subCommands = commandHandlers.ToImmutableDictionary();
            HelpText = helpBuilder.ToString();
        }

        protected sealed override async Task HandleCommandAsync(CommandContext context, string command,
            string[] parameters, IEnumerable<ChatFormat> chatLinks)
        {
            if (parameters.Length > 0)
            {
                if (!subCommands.TryGetValue(parameters[0], out SubCommandHandler commandCallback))
                {
                    await SendHelpAsync(context);
                    return;
                }

                await (commandCallback?.Invoke(context, parameters[0], parameters.Skip(1).ToArray(), chatLinks) ??
                      Task.CompletedTask);
            }
            else
                await SendHelpAsync(context);

            // TODO
        }

        private delegate Task SubCommandHandler(CommandContext context, string subCommand, string[] parameters, IEnumerable<ChatFormat> chatLinks);
    }
}
