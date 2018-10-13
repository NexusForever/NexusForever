using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Command;
using NexusForever.WorldServer.Network.Message.Model;
using NLog;
using System;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class SocialHandler
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public static readonly string CommandPrefix = "!";

        [MessageHandler(GameMessageOpcode.ClientChat)]
        public static void HandleChat(WorldSession session, ClientChat chat)
        {
            if (chat.Message.StartsWith(CommandPrefix))
            {
                try
                {
                    CommandManager.ParseCommand(chat.Message, out string command, out string[] parameters);
                    CommandHandlerDelegate handler = CommandManager.GetCommandHandler(command);
                    handler?.Invoke(session, parameters);
                } catch (Exception e)
                {
                    log.Warn(e.Message);
                }
            }
        }
    }
}
