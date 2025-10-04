using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Social;
using NexusForever.Network.Session;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Abstract.Chat
{
    public interface IGlobalChatManager
    {
        /// <summary>
        /// Process and delegate a <see cref="ClientChat"/> message from <see cref="IPlayer"/>.
        /// </summary>
        void HandleClientChat(IPlayer player, ClientChat chat);

        /// <summary>
        /// Handle's whisper messages between 2 clients
        /// </summary>
        void HandleWhisperChat(IPlayer player, ClientChatWhisper whisper);

        void SendMessage(IGameSession session, string message, string name = "", ChatChannelType type = ChatChannelType.System);
    }
}
