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

        public HelpCommandHandler(ILogger<HelpCommandHandler> logger, IServiceProvider serviceProvider) : base(new[] { "help", "h", "?"}, false, logger)
        {
            ServiceProvider = serviceProvider;
        }

        protected override void HandleCommand(CommandContext context, string c, string[] parameters)
        {
            var commandHandlers = ServiceProvider.GetServices<ICommandHandler>();
            var allCommands = commandHandlers.Where(i => i.GetType() != GetType()).SelectMany(i => i.GetCommands().Select(x => new { Command = x, Handler = i })).ToList();
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Nexus Forever - Command List");
            foreach (var group in allCommands.GroupBy(x => GetModuleName(x.Handler), x => x.Command))
            {
                stringBuilder.Append("-- ").AppendLine(group.Key);
                foreach (var command in group.OrderBy(i => i))
                {
                    stringBuilder.Append("   ").AppendLine(command);
                }

                stringBuilder.AppendLine();
            }

            stringBuilder.AppendLine("To get more help for an individual command, type <command> help");
            context.SendMessage(Logger, stringBuilder.ToString());
        }

        private string GetModuleName(object obj)
        {
            var type = obj.GetType();
            return type.GetCustomAttribute<NameAttribute>()?.Name ?? type.Name;
            
        }
    }
}
