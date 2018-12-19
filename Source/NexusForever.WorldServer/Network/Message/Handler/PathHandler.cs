using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Network.Message.Model;
using NLog;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class PathHandler
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        [MessageHandler(GameMessageOpcode.ClientPathActivate)]
        public static void HandlePathActivate(WorldSession session, ClientPathActivate clientPathActivate)
        {
            log.Debug($"ClientPathActivate: Path: {clientPathActivate.Path}, UseTokens: {clientPathActivate.UseTokens}");
            Player player = session.Player;

            if (clientPathActivate.UseTokens)
            {
                // TODO: Implement block if not enough tokens
                // TODO: Remove tokens from account and send relevant packet updates
            }

            if(player.PathManager.ActivatePath(clientPathActivate.Path))
            {
                UpdatePathPackets(session, player);
            }

            // TODO: Handle errors
        }

        [MessageHandler(GameMessageOpcode.ClientPathUnlock)]
        public static void HandlePathUnlock(WorldSession session, ClientPathUnlock clientPathUnlock)
        {
            log.Debug($"ClientPathActivate: Path: {clientPathUnlock.Path}");

            Player player = session.Player;
            byte Result = 0;

            // TODO: Handle removing service tokens
            // TODO: Return proper error codes


            // TODO: HasEnoughTokens should be a request to a currency manager of somesort
            bool HasEnoughTokens = true;
            if(HasEnoughTokens)
            {
                if(player.PathManager.UnlockPath(clientPathUnlock.Path))
                {
                    Result = 1;
                } else
                {
                    // TODO: Return failure result
                    Result = 2;
                }

                session.EnqueueMessageEncrypted(new ServerPathUnlockResult
                {
                    Result = Result,
                    UnlockedPathMask = player.Path.PathsUnlocked
                });

                if(Result == 1)
                {
                    UpdatePathPackets(session, player);
                }
            }
        }

        /// <summary>
        /// Used to update shared path packets between certain functions
        /// </summary>
        /// <param name="session"></param>
        /// <param name="player"></param>
        public static void UpdatePathPackets(WorldSession session, Player player)
        {
            player.PathManager.SendSetUnitPathTypePacket();
            player.PathManager.SendPathLogPacket();
        }
    }
}
