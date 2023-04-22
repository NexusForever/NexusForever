﻿using NexusForever.Cryptography;
using NexusForever.Database;
using NexusForever.Database.Auth;
using NexusForever.Game.Configuration.Model;
using NexusForever.Game.Static.RBAC;
using NexusForever.Shared.Configuration;
using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Command.Convert;
using NexusForever.WorldServer.Command.Static;

namespace NexusForever.WorldServer.Command.Handler
{
    [Command(Permission.Account, "A collection of commands to modify game accounts.", "acc", "account")]
    public class AccountCommandCategory : CommandCategory
    {
        [Command(Permission.AccountCreate, "Create a new account.", "create")]
        public void HandleAccountCreate(ICommandContext context,
            [Parameter("Email address for the new account", converter: typeof(StringLowerParameterConverter))]
            string email,
            [Parameter("Password for the new account")]
            string password,
            [Parameter("Role", ParameterFlags.Optional, typeof(EnumParameterConverter<Role>))]
            Role? role = null)
        {
            if (DatabaseManager.Instance.GetDatabase<AuthDatabase>().AccountExists(email))
            {
                context.SendMessage("Account with that username already exists. Please try another.");
                return;
            }

            role ??= (SharedConfiguration.Instance.Get<RealmConfig>().DefaultRole ?? Role.Player);

            (string salt, string verifier) = PasswordProvider.GenerateSaltAndVerifier(email, password);
            DatabaseManager.Instance.GetDatabase<AuthDatabase>().CreateAccount(email, salt, verifier, (uint)role);

            context.SendMessage($"Account {email} created successfully");
        }

        [Command(Permission.AccountDelete, "Delete an account.", "delete")]
        public void HandleAccountDelete(ICommandContext context,
            [Parameter("Email address of the account to delete")]
            string email)
        {
            if (DatabaseManager.Instance.GetDatabase<AuthDatabase>().DeleteAccount(email))
                context.SendMessage($"Account {email} successfully removed!");
            else
                context.SendMessage($"Cannot find account with Email: {email}");
        }
    }
}
