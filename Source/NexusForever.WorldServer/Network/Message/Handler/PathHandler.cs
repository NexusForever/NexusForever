using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class PathHandler
    {
        [MessageHandler(GameMessageOpcode.ClientPathActivate)]
        public static void HandlePing(WorldSession session, ClientPathActivate clientPathActivate)
        {
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
