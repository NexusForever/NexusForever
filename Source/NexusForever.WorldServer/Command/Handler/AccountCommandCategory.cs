using NexusForever.Shared.Cryptography;
using NexusForever.Shared.Database;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.RBAC.Static;

namespace NexusForever.WorldServer.Command.Handler
{
    [Command(Permission.Account, "A collection of commands to modify game accounts.", "acc", "account")]
    public class AccountCommandCategory : CommandCategory
    {
        [Command(Permission.AccountCreate, "Create a new account.", "create")]
        public void HandleAccountCreate(ICommandContext context,
            [Parameter("Email address for the new account")]
            string email,
            [Parameter("Password for the new account")]
            string password)
        {
            email = email.ToLower();

            if (DatabaseManager.Instance.AuthDatabase.AccountExists(email))
            {
                context.SendMessage("Account with that username already exists. Please try another.");
                return;
            }

            (string salt, string verifier) = PasswordProvider.GenerateSaltAndVerifier(email, password);
            DatabaseManager.Instance.AuthDatabase.CreateAccount(email, salt, verifier);

            context.SendMessage($"Account {email} created successfully");
        }

        [Command(Permission.AccountDelete, "Delete an account.", "delete")]
        public void HandleAccountDelete(ICommandContext context,
            [Parameter("Email address of the account to delete")]
            string email)
        {
            if (DatabaseManager.Instance.AuthDatabase.DeleteAccount(email))
                context.SendMessage($"Account {email} successfully removed!");
            else
                context.SendMessage($"Cannot find account with Email: {email}");
        }

        [Command(Permission.AccountInventory, "A collection of commands to manage account inventory.", "inventory")]
        public class AccountInventoryCommandCategory : CommandCategory
        {
            [Command(Permission.AccountInventoryItemAdd, "Add an item to the account inventory.", "add")]
            public void HandleAccountInventoryCommandItemAdd(ICommandContext context,
                [Parameter("Item to add")]
                uint itemId,
                [Parameter("Item quantity")]
                uint? quantity)
            {
                quantity ??= 1u;

                if (context.GetTargetOrInvoker<Player>() == null)
                {
                    context.SendMessage("You need to have a target to add an Account Item to!");
                    return;
                }

                AccountItemEntry accountItem = GameTableManager.Instance.AccountItem.GetEntry(itemId);
                if (accountItem == null)
                {
                    context.SendMessage($"Could not find Account Item with ID {itemId}. Please try again with a valid ID.");
                    return;
                }

                context.GetTargetOrInvoker<Player>().Session.AccountInventory.ItemCreate(accountItem);
            }
        }
    }
}
