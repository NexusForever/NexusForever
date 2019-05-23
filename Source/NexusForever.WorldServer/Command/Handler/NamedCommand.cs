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
using NexusForever.WorldServer.Game.Account;

namespace NexusForever.WorldServer.Command.Handler
{
    public abstract class NamedCommand : CommandHandlerBase
    {
        public override int Order { get; } = 100;
        public override Permission RequiredPermission { get; } = Permission.Everything;
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
                RequiredPermission = nameAttribute.PermissionRequired;

                Logger.Trace($"{nameAttribute.Name} default permission: {RequiredPermission}");
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

            bool isConsole = session.Session == null;

            if (isConsole || RoleManager.HasPermission(session.Session, RequiredPermission))
                await HandleCommandAsync(session, command, parameters);
            else
                await session.SendMessageAsync($"Your account status is too low for this command: !{command} (Requires permission: {RequiredPermission})");
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
