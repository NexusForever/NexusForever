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
            try
            {
                AuthDatabase.CreateAccount(parameters[0], parameters[1]);
                log.Info($"Account {parameters[0]} succesfully created!");
            }
            catch (Exception exception)
            {
                log.Error(exception);
            }
        }
    }
}
