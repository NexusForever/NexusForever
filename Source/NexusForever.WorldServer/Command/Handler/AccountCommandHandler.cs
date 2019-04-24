using System.Collections.Generic;
using System.Threading.Tasks;
using NexusForever.Shared.Database.Auth;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Network.Message.Model.Shared;

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
        public async Task HandleAccountCreate(CommandContext context, string subCommand, string[] parameters, IEnumerable<ChatFormat> chatLinks)
        {
            if (parameters.Length < 2)
            {
                await SendHelpAsync(context).ConfigureAwait(false);
                return;
            }

            AuthDatabase.CreateAccount(parameters[0], parameters[1]);
            await context.SendMessageAsync($"Account {parameters[0]} created successfully")
                .ConfigureAwait(false);
        }

        [SubCommandHandler("delete", "email - Delete an account")]
        public async Task HandleAccountDeleteAsync(CommandContext context, string subCommand, string[] parameters, IEnumerable<ChatFormat> chatLinks)
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
