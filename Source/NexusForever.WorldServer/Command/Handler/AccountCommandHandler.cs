using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NexusForever.Shared.Database.Auth;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Account Management")]
    public class AccountCommandHandler : CommandCategory
    {
        public AccountCommandHandler(ILogger<AccountCommandHandler> logger)
            : base(new[] {"acc", "account"}, false, logger)
        {
        }

        [SubCommandHandler("create", "email password - Create a new account")]
        public async Task HandleAccountCreate(CommandContext context, string subCommand, string[] parameters)
        {
            AuthDatabase.CreateAccount(parameters[0], parameters[1]);
            await context.SendMessageAsync(Logger, $"Account {parameters[0]} created successfully")
                .ConfigureAwait(false);
        }

        [SubCommandHandler("delete", "email - Delete an account")]
        public async Task HandleAccountDeleteAsync(CommandContext context, string subCommand, string[] parameters)
        {
            if (AuthDatabase.DeleteAccount(parameters[0]))
                await context.SendMessageAsync(Logger, $"Account {parameters[0]} successfully removed!")
                    .ConfigureAwait(false);
            else
                await context.SendMessageAsync(Logger, $"Cannot find account with Email: {parameters[0]}")
                    .ConfigureAwait(false);
        }
    }
}
