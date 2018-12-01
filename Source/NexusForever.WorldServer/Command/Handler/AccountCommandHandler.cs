using System;
using Microsoft.Extensions.Logging;
using NexusForever.Shared.Database.Auth;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Network;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Account Management")]
    public class AccountCommandHandler : CommandCategory
    {
        public AccountCommandHandler(ILogger<AccountCommandHandler> logger) : base(new[] { "acc", "account" }, false, logger) { }

        [SubCommandHandler("create", "email password - Create a new account")]
        public void HandleAccountCreate(CommandContext context, string subCommand, string[] parameters)
        {
            AuthDatabase.CreateAccount(parameters[0], parameters[1]);
            context.SendMessage(Logger, $"Account {parameters[0]} created successfully");
        }

        [SubCommandHandler("delete", "email - Delete an account")]
        public void HandleAccountDelete(CommandContext context, string subCommand, string[] parameters)
        {
            if (AuthDatabase.DeleteAccount(parameters[0]))
            {
                //Logger.LogInformation("Account {accountName} successfully removed!", parameters[0]);
                context.SendMessage(Logger, $"Account {parameters[0]} successfully removed!");
            }
            else
            {
                //Logger.LogWarning("Cannot find account with Email: {accountName}", parameters[0]);
                context.SendMessage(Logger, $"Cannot find account with Email: {parameters[0]}");
            }
        }
    }
}