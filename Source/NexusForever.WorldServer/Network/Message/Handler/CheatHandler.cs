using System;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Command;
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
                CommandManager.Instance.HandleCommand(session, cheat.Message, false);
            }
            catch (Exception e)
            {
                log.Warn(e.Message);
            }
        }
    }
}
