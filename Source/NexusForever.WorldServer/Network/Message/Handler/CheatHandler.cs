using System;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.WorldServer.Command;
using NexusForever.WorldServer.Command.Context;
using NLog;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class CheatHandler
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        [MessageHandler(GameMessageOpcode.ClientCheat)]
        public static void HandleCheat(IWorldSession session, ClientCheat cheat)
        {
            try
            {
                var target  = session.Player.GetVisible<IWorldEntity>(session.Player.TargetGuid);
                var context = new WorldSessionCommandContext(session, target);
                CommandManager.Instance.HandleCommand(context, cheat.Message);
            }
            catch (Exception e)
            {
                log.Warn(e.Message);
            }
        }
    }
}
