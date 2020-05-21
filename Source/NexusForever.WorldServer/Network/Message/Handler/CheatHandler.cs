using System;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Command;
using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Network.Message.Model;
using NLog;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class CheatHandler
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        [MessageHandler(GameMessageOpcode.ClientCheat)]
        public static void HandleCheat(WorldSession session, ClientCheat cheat)
        {
            try
            {
                var target  = session.Player.GetVisible<WorldEntity>(session.Player.TargetGuid);
                var context = new WorldSessionCommandContext(session.Player, target);
                CommandManager.Instance.HandleCommand(context, cheat.Message);
            }
            catch (Exception e)
            {
                log.Warn(e.Message);
            }
        }
    }
}
