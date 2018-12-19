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
