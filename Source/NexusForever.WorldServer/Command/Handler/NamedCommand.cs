using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Game.Account.Static;
using NexusForever.Shared.Configuration;
using Microsoft.Extensions.Configuration;

namespace NexusForever.WorldServer.Command.Handler
{
    public abstract class NamedCommand : CommandHandlerBase
    {
        public override int Order { get; } = 100;
        public override int MinimumStatus { get; } = Enum.GetValues(typeof(AccountStatus)).Cast<int>().Last();
        public virtual string HelpText { get; }
        public bool SupportsHelp => !string.IsNullOrWhiteSpace(HelpText);

        public ImmutableArray<string> CommandNames { get; }

        public bool RequiresSession { get; }

        protected NamedCommand(bool requiresSession, params string[] commandNames)
        {
            CommandNames = commandNames.ToImmutableArray();
            RequiresSession = requiresSession;

            var NameAttributeValue = GetType().GetCustomAttributes(typeof(NameAttribute), true);
            if (NameAttributeValue.Length > 0)
            {
                var nameAttribute = (NameAttribute)NameAttributeValue[0];
                string command = nameAttribute.Name.Replace(" ", string.Empty);

                IEnumerable<KeyValuePair<string, string>> commandConfiguration = ConfigurationManager<WorldServerConfiguration>.GetConfiguration().GetSection("Commands").AsEnumerable();
                foreach(var section in commandConfiguration)
                {
                    var sectionKey = section.Key.Replace(" ", string.Empty).Split(":");
                    // The below IF statement checks if this Key is for this NamedCommand
                    if (sectionKey.Contains(command) && sectionKey.Contains("MinimumStatus") && sectionKey.Length - 1 == 3)
                        MinimumStatus = int.Parse(section.Value);
                }
            }
    }

        public override IEnumerable<string> GetCommands()
        {
            return CommandNames;
        }

        public sealed override async Task HandleAsync(CommandContext session, string text)
        {
            ParseCommand(text, out string command, out string[] parameters);
            if (SupportsHelp && parameters.Length != 0 && IsHelpRequest(parameters[0]))
            {
                await SendHelpAsync(session).ConfigureAwait(false);
                return;
            }

            if (HasPermission(session.Session.Account.Status))
                await HandleCommandAsync(session, command, parameters);
            else
                await session.SendMessageAsync($"Your account status is too low for this command: !{command} {string.Join(' ', parameters)} ({session.Session.Account.Status} | {MinimumStatus})");
        }

        private bool IsHelpRequest(string text)
        {
            string[] helpVerbs =
            {
                "help",
                "?"
            };

            return helpVerbs.Any(i => string.Equals(i, text, StringComparison.OrdinalIgnoreCase));
        }

        protected abstract Task HandleCommandAsync(CommandContext session, string command, string[] parameters);

        public override Task<bool> HandlesAsync(CommandContext session, string input)
        {
            /*if (RequiresSession && session.Session == null)
                return false;*/

            ParseCommand(input, out string command, out string[] parameters);
            return Task.FromResult(CommandNames.Any(i => string.Equals(command, i, StringComparison.OrdinalIgnoreCase))
                                   && (parameters.Length != 0 && IsHelpRequest(parameters[0]) ||
                                       RequiresSession && session.Session != null || !RequiresSession));
        }

        public virtual Task SendHelpAsync(CommandContext session)
        {
            return session.SendMessageAsync(HelpText);
        }
    }
}
