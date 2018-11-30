using System;
using NexusForever.Shared.Database.Auth;
using NexusForever.WorldServer.Network;
using NLog;

namespace NexusForever.WorldServer.Command.Handler
{
    public static class AccountHandler
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        [CommandHandler("accountcreate")]
        public static void HandleAccountCreate(WorldSession session, string[] parameters)
        {
            AuthDatabase.CreateAccount(parameters[0], parameters[1]);
            log.Info($"Account {parameters[0]} succesfully created!");
        }

        [CommandHandler("accountdelete")]
        public static void HandleAccountDelete(WorldSession session, string[] parameters)
        {
            if (AuthDatabase.DeleteAccount(parameters[0]))
                log.Info($"Account {parameters[0]} succesfully removed!");
            else
                log.Error($"Cannot find account with Email : {parameters[0]}");
        }
    }
}
