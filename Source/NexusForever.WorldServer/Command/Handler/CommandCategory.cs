using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Logging;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Network;

namespace NexusForever.WorldServer.Command.Handler
{
    public abstract class CommandCategory : NamedCommand
    {
        private readonly string _dynamicHelpText;
        public override string HelpText => _dynamicHelpText;
        private delegate void SubCommandHandler(CommandContext context, string subCommand, string[] parameters);
        private readonly ImmutableDictionary<string, SubCommandHandler> _subCommands;
        protected CommandCategory(string categoryName, bool requiresSession, ILogger logger) : this(new[] { categoryName }, requiresSession, logger) { }
        protected CommandCategory(IEnumerable<string> categoryNames, bool requiresSession, ILogger logger) : base(categoryNames, requiresSession, logger)
        {
            StringBuilder helpBuilder = new StringBuilder();
            helpBuilder.AppendLine("--- sub commands");

            Dictionary<string, SubCommandHandler> commandHandlers = new Dictionary<string, SubCommandHandler>(StringComparer.OrdinalIgnoreCase);
            foreach (MethodInfo method in GetType().GetMethods().Where(i => !i.IsStatic && !i.IsAbstract))
            {
                // Support for multiple attributes.
                foreach (var attribute in method.GetCustomAttributes<SubCommandHandlerAttribute>())
                {
                    if (attribute == null)
                        continue;

                    ParameterInfo[] parameterInfo = method.GetParameters();

                    #region Debug
                    Debug.Assert(parameterInfo.Length == 3);
                    Debug.Assert(typeof(CommandContext) == parameterInfo[0].ParameterType);
                    Debug.Assert(typeof(string) == parameterInfo[1].ParameterType);
                    Debug.Assert(typeof(string[]) == parameterInfo[2].ParameterType);
                    #endregion


                    helpBuilder.Append($"   {attribute.Command} - ");
                    if (string.IsNullOrWhiteSpace(attribute.HelpText))
                    {
                        helpBuilder.AppendLine("No help available.");
                    }
                    else
                    {
                        helpBuilder.AppendLine(attribute.HelpText);
                    }
                    commandHandlers.Add(attribute.Command, (SubCommandHandler)Delegate.CreateDelegate(typeof(SubCommandHandler), this, method));
                }
            }

            _subCommands = commandHandlers.ToImmutableDictionary();
            _dynamicHelpText = helpBuilder.ToString();
        }

        protected sealed override void HandleCommand(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length > 0)
            {
                _subCommands.TryGetValue(parameters[0], out var commandCallback);
                commandCallback?.Invoke(context, parameters[0], parameters.Skip(1).ToArray());
            }
            // TODO
        }
    }
}