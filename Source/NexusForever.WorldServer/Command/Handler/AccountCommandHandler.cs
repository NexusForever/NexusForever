using System.Threading.Tasks;
using NexusForever.Shared.Database.Auth;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Account Management")]
    public class AccountCommandHandler : CommandCategory
    {
        public AccountCommandHandler()
            : base(false, "acc", "account")
        {
        }

        [SubCommandHandler("create", "email password - Create a new account")]
        public async Task HandleAccountCreate(CommandContext context, string subCommand, string[] parameters)
        {
            if (parameters.Length < 2)
            {
                await SendHelpAsync(context).ConfigureAwait(false);
                return;
            }

            bool success = AuthDatabase.CreateAccount(parameters[0], parameters[1]);
            if (success)
                await context.SendMessageAsync($"Account {parameters[0]} created successfully")
                    .ConfigureAwait(false);
            else
                await context.SendMessageAsync($"Account {parameters[0]} could not be created. Duplicate username.")
                    .ConfigureAwait(false);
        }

        [SubCommandHandler("delete", "email - Delete an account")]
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
