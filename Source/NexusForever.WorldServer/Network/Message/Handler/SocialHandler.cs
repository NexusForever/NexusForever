using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Command;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class SocialHandler
    {
        [MessageHandler(GameMessageOpcode.ClientChat)]
        public static void HandleChat(WorldSession session, ClientChat chat)
        {
            if (chat.Message.StartsWith("!"))
            {
                CommandManager.ParseCommand(chat.Message, out string command, out string[] parameters);
                CommandHandlerDelegate handler = CommandManager.GetCommandHandler(command);
                handler?.Invoke(session, parameters);
            }
        }
    }
}
