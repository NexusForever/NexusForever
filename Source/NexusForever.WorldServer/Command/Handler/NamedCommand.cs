using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using NexusForever.WorldServer.Command.Contexts;

namespace NexusForever.WorldServer.Command.Handler
{
    public abstract class NamedCommand : CommandHandlerBase
    {

        public override int Order { get; } = 100;
        public virtual string HelpText { get; }
        public bool SupportsHelp => !string.IsNullOrWhiteSpace(HelpText);

        public ImmutableArray<string> CommandNames { get; }

        public bool RequiresSession { get; }

        protected NamedCommand(bool requiresSession, params string[] commandNames)
        {
            CommandNames = commandNames.ToImmutableArray();
            RequiresSession = requiresSession;
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

            await HandleCommandAsync(session, command, parameters);
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

        public virtual Task SendHelpAsync(CommandContext context)
        {
            return context.SendMessageAsync(HelpText);
        }
    }
}
