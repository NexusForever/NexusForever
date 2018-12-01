using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Extensions.Logging;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Network;

namespace NexusForever.WorldServer.Command.Handler
{
    public abstract class NamedCommand : CommandHandlerBase
    {
        protected NamedCommand(IEnumerable<string> commandNames, bool requiresSession, ILogger logger) : base(logger)
        {
            CommandNames = commandNames.ToImmutableArray();
            RequiresSession = requiresSession;
        }

        protected NamedCommand(string commandName, bool requiresSession, ILogger logger) : this(new[] { commandName }, requiresSession, logger)
        {

        }

        public override IEnumerable<string> GetCommands()
        {
            return CommandNames;
        }

        public override int Order { get; } = 100;
        public virtual string HelpText { get; }
        public bool SupportsHelp => !string.IsNullOrWhiteSpace(HelpText);

        public sealed override void Handle(CommandContext session, string text)
        {
            ParseCommand(text, out var command, out var parameters);
            if (SupportsHelp && parameters.Length > 0 && IsHelpRequest(parameters[0]))
            {
                GetHelp(session);
                return;
            }

            HandleCommand(session, command, parameters);
        }

        private bool IsHelpRequest(string text)
        {
            string[] helpVerbs = new[]
            {
                "help",
                "?"
            };
            if (helpVerbs.Any(i => string.Equals(i, text, StringComparison.OrdinalIgnoreCase))) return true;
            return false;
        }

        protected abstract void HandleCommand(CommandContext session, string command, string[] parameters);

        public override bool Handles(CommandContext session, string input)
        {
            if (RequiresSession && session.Session == null) return false;
            ParseCommand(input, out var command, out _);
            return CommandNames.Any(i => string.Equals(command, i, StringComparison.OrdinalIgnoreCase));
        }
        public ImmutableArray<string> CommandNames { get; }
        [Obsolete("Please user CommandNames instead", true)]
        public string CommandName { get; }
        public bool RequiresSession { get; }

        public virtual string GetHelp(CommandContext session)
        {
            session.SendMessage(Logger, HelpText);
            return HelpText;
        }
    }
}