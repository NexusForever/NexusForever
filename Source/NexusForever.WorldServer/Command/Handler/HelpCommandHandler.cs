using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Network;

namespace NexusForever.WorldServer.Command.Handler
{
    public class HelpCommandHandler : NamedCommand
    {
        public IServiceProvider ServiceProvider { get; }

        public HelpCommandHandler(ILogger<HelpCommandHandler> logger, IServiceProvider serviceProvider) : base(new[] { "help", "h", "?" }, false, logger)
        {
            ServiceProvider = serviceProvider;
        }

        protected override void HandleCommand(CommandContext context, string c, string[] parameters)
        {

            var commandHandlers = ServiceProvider.GetServices<ICommandHandler>();
            var allCommands = commandHandlers.Where(i => i.GetType() != GetType()).SelectMany(i => i.GetCommands().Select(x => new { Command = x, Handler = i })).ToList();
            if (parameters.Length != 1 || !allCommands.Any(x => string.Equals(x.Command, parameters[0], StringComparison.OrdinalIgnoreCase)))
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("Nexus Forever - Command List");
                foreach (var group in allCommands.GroupBy(x => GetModuleName(x.Handler), x => x.Command))
                {
                    // TODO: Get help text from the module, if available.
                    stringBuilder.Append("-- ").AppendLine(group.Key);
                    foreach (var command in group.OrderBy(i => i))
                    {
                        stringBuilder.Append("   ").AppendLine(command);
                    }

                    stringBuilder.AppendLine();
                }

                stringBuilder.AppendLine("To get more help for an individual command, type help <command>");
                context.SendMessage(Logger, stringBuilder.ToString());
            }
            else
            {
                CommandManager.HandleCommand(context, $"{parameters[0]} help", false);
            }
        }

        private string GetModuleName(object obj)
        {
            var type = obj.GetType();
            return type.GetCustomAttribute<NameAttribute>()?.Name ?? type.Name;
        }
    }
}
