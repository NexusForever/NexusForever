using System.Threading.Tasks;
using NexusForever.Shared.Cryptography;
using NexusForever.Shared.Database;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.GameTable.Static;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Game.Account.Static;

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

            (string salt, string verifier) = PasswordProvider.GenerateSaltAndVerifier(parameters[0], parameters[1]);
            DatabaseManager.Instance.AuthDatabase.CreateAccount(parameters[0], salt, verifier);

            await context.SendMessageAsync($"Account {parameters[0]} created successfully")
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

            if (DatabaseManager.Instance.AuthDatabase.DeleteAccount(parameters[0]))
                await context.SendMessageAsync($"Account {parameters[0]} successfully removed!")
                    .ConfigureAwait(false);
            else
                await context.SendMessageAsync($"Cannot find account with Email: {parameters[0]}")
                    .ConfigureAwait(false);
        }

        [SubCommandHandler("currencyadd", "currencyId amount - Add the amount provided to the currencyId provided")]
        public Task HandleAccountCurrencyAdd(CommandContext context, string command, string[] parameters)
        {
            if (context.Session.Account == null)
            {
                context.SendMessageAsync("Account not found. Please try again.");
                return Task.CompletedTask;
            }

            if (parameters.Length != 2)
            {
                context.SendMessageAsync("Parameters are invalid. Please try again.");
                return Task.CompletedTask;
            }

            bool currencyParsed = byte.TryParse(parameters[0], out byte currencyId);
            if (!currencyParsed || currencyId == 13) // Disabled Character Token for now due to causing server errors if the player tries to use it. TODO: Fix level 50 creation
            {
                context.SendMessageAsync("Invalid currencyId. Please try again.");
                return Task.CompletedTask;
            }

            AccountCurrencyTypeEntry currencyEntry = GameTableManager.Instance.AccountCurrencyType.GetEntry(currencyId);
            if (currencyEntry == null)
            {
                context.SendMessageAsync("Invalid currencyId. Please try again.");
                return Task.CompletedTask;
            }

            if (!uint.TryParse(parameters[1], out uint amount))
            {
                context.SendMessageAsync("Unable to parse amount. Please try again.");
                return Task.CompletedTask;
            }

            context.Session.AccountCurrencyManager.CurrencyAddAmount((AccountCurrencyType)currencyId, amount);
            return Task.CompletedTask;
        }

        [SubCommandHandler("currencylist", "List all account currencies")]
        public Task HandleAccountCurrencyList(CommandContext context, string command, string[] parameters)
        {
            var tt = GameTableManager.Instance.GetTextTable(Language.English);
            foreach (var entry in GameTableManager.Instance.AccountCurrencyType.Entries)
            {
                context.SendMessageAsync($"ID {entry.Id}: {tt.GetEntry(entry.LocalizedTextId)}");
            }

            return Task.CompletedTask;
        }
    }
}
