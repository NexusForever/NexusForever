using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;

namespace NexusForever.WorldServer.Command.Handler
{
    public class HelpCommandHandler : NamedCommand
    {

        public HelpCommandHandler()
            : base(false, "help", "h", "?")
        {
        }

        protected override async Task HandleCommandAsync(CommandContext context, string c, string[] parameters)
        {
            IEnumerable<ICommandHandler> commandHandlers = CommandManager.Instance.GetCommandHandlers();
            var allCommands = commandHandlers
                .Where(i => i.GetType() != GetType())
                .SelectMany(i => i.GetCommands()
                    .Select(x => new {Command = x, Handler = i}))
                .ToList();

            if (parameters.Length != 1 || !allCommands.Any(x =>
                    string.Equals(x.Command, parameters[0], StringComparison.OrdinalIgnoreCase)))
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("Nexus Forever - Command List");

                foreach (IGrouping<string, string> group in allCommands.GroupBy(x => GetModuleName(x.Handler),
                    x => x.Command))
                {
                    // TODO: Get help text from the module, if available.
                    stringBuilder.Append("-- ").AppendLine(group.Key);
                    foreach (string command in group.OrderBy(i => i))
                        stringBuilder.Append("   ").AppendLine(command);

                    stringBuilder.AppendLine();
                }

                stringBuilder.AppendLine("To get more help for an individual command, type help <command>");
                await context.SendMessageAsync(stringBuilder.ToString()).ConfigureAwait(false);
            }
            else
                await CommandManager.Instance.HandleCommandAsync(context, $"{parameters[0]} help", false).ConfigureAwait(false);
        }

        private string GetModuleName(object obj)
        {
            Type type = obj.GetType();
            return type.GetCustomAttribute<NameAttribute>()?.Name ?? type.Name;
        }
    }
}
