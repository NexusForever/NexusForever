using System.Collections.Generic;
using System.Threading.Tasks;
using NexusForever.Shared.Configuration;
using NexusForever.Shared.Database.Auth;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Game.Account.Static;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Account Management", Permission.None)]
    public class AccountCommandHandler : CommandCategory
    {
        public AccountCommandHandler()
            : base(false, "acc", "account")
        {
        }

        [SubCommandHandler("create", "email password [extraRoles] - Create a new account", Permission.CommandAccountCreate)]
        public async Task HandleAccountCreate(CommandContext context, string subCommand, string[] parameters)
        {
            if (parameters.Length < 2)
            {
                await SendHelpAsync(context).ConfigureAwait(false);
                return;
            }

            List<ulong> extraRoles = new List<ulong>();
            for (int i = 2; i < parameters.Length; i++)
                extraRoles.Add(ulong.Parse(parameters[i]));

            AuthDatabase.CreateAccount(parameters[0], parameters[1], defaultRole: ConfigurationManager<WorldServerConfiguration>.Config.DefaultRole, extraRoles.ToArray());
            await context.SendMessageAsync($"Account {parameters[0]} created successfully")
                .ConfigureAwait(false);
        }

        [SubCommandHandler("delete", "email - Delete an account", Permission.CommandAccountDelete)]
        public async Task HandleAccountDeleteAsync(CommandContext context, string subCommand, string[] parameters)
        {
            if (parameters.Length < 1)
            {
                await SendHelpAsync(context).ConfigureAwait(false);
                return;
            }

            if (AuthDatabase.DeleteAccount(parameters[0]))
                await context.SendMessageAsync($"Account {parameters[0]} successfully removed!")
                    .ConfigureAwait(false);
            else
                await context.SendMessageAsync($"Cannot find account with Email: {parameters[0]}")
                    .ConfigureAwait(false);
        }
    }
}
