using System;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Command;
using NexusForever.WorldServer.Network.Message.Model;
using NLog;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class CheatHandler
    {
        private static Logger Log { get; } = LogManager.GetCurrentClassLogger();
        [MessageHandler(GameMessageOpcode.ClientCheat)]
        public static void HandleCheat(WorldSession session, ClientCheat cheat)
        {
            try
            {
                CommandManager.HandleCommand(session, cheat.Message, false);
                //CommandManager.ParseCommand(chat.Message, out string command, out string[] parameters);
                //CommandHandlerDelegate handler = CommandManager.GetCommandHandler(command);
                //handler?.Invoke(session, parameters);
            }
            catch (Exception e)
            {
                Log.Warn(e.Message);
            }
        }
    }
}