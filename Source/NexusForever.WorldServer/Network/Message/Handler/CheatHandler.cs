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
                IWorldEntity target = null;
                if (session.Player.TargetGuid != null)
                    target = session.Player.GetVisible<IWorldEntity>(session.Player.TargetGuid.Value);

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
