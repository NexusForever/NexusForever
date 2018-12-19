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

            if(clientPathActivate.UseTokens)
            {
                // TODO: Implement block if not enough tokens
                // TODO: Remove tokens from account and send relevant packet updates
            }

            Player player = session.Player;
            player.Path.ActivePath = clientPathActivate.Path;

            UpdatePathPackets(session, player);
        }

        [MessageHandler(GameMessageOpcode.ClientPathUnlock)]
        public static void HandlePathUnlock(WorldSession session, ClientPathUnlock clientPathUnlock)
        {
            log.Debug($"ClientPathActivate: Path: {clientPathUnlock.Path}");

            Player player = session.Player;

            // TODO: Handle removing service tokens
            // TODO: Confirm that it's not already unlocked and return proper error codes if it is
            // TODO: Extend PathManager to modify paths, unlocked paths
            player.Path.PathsUnlocked |= player.PathManager.CalculatePathUnlocked((byte)clientPathUnlock.Path);

            session.EnqueueMessageEncrypted(new ServerPathUnlockResult
            {
                Result = 1,
                UnlockedPathMask = player.Path.PathsUnlocked
            });

            UpdatePathPackets(session, player);
        }

        /// <summary>
        /// Used to update shared path packets between certain functions
        /// </summary>
        /// <param name="session"></param>
        /// <param name="player"></param>
        public static void UpdatePathPackets(WorldSession session, Player player)
        {
            session.EnqueueMessageEncrypted(new ServerSetUnitPathType
            {
                Guid = player.Guid,
                Path = player.Path.ActivePath,
            });
            session.EnqueueMessageEncrypted(new ServerPathLog
            {
                ActivePath = player.Path.ActivePath,
                PathProgress = new ServerPathLog.Progress
                {
                    Soldier = player.Path.SoldierXp,
                    Settler = player.Path.SettlerXp,
                    Scientist = player.Path.ScientistXp,
                    Explorer = player.Path.ExplorerXp
                },
                UnlockedPathMask = player.Path.PathsUnlocked
            });
        }
    }
}
