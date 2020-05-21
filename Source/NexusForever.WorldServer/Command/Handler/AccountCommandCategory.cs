using NexusForever.Shared.Cryptography;
using NexusForever.Shared.Database;
using NexusForever.WorldServer.Command.Context;
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
    }
}
