using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Social;
using NexusForever.Network;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Abstract.Social
{
    public interface IGlobalChatManager
    {
        void Initialise();

        /// <summary>
        /// Create a new <see cref="IChatChannel"/> with supplied <see cref="ChatChannelType"/>.
        /// </summary>
        IChatChannel CreateChatChannel(ChatChannelType type, string name, string password = null);

        /// <summary>
        /// Create a new <see cref="IChatChannel"/> with supplied <see cref="ChatChannelType"/> and id.
        /// </summary>
        IChatChannel CreateChatChannel(ChatChannelType type, ulong chatId, string name, string password = null);

        /// <summary>
        /// Returns an existing <see cref="IChatChannel"/> with supplied <see cref="ChatChannelType"/> and id.
        /// </summary>
        IChatChannel GetChatChannel(ChatChannelType type, string name);

        /// <summary>
        /// Returns an existing <see cref="IChatChannel"/> with supplied <see cref="ChatChannelType"/> and name.
        /// </summary>
        IChatChannel GetChatChannel(ChatChannelType type, ulong id);

        /// <summary>
        /// Returns a collection of <see cref="IChatChannel"/>'s in which supplied character id belongs to.
        /// </summary>
        /// <remarks>
        /// This should only be used in situations where the local manager is not accessible for a character.
        /// </remarks>
        IEnumerable<IChatChannel> GetCharacterChatChannels(ChatChannelType type, ulong characterId);

        /// <summary>
        /// Track a new chat channel for the supplied character.
        /// </summary>
        /// <remarks>
        /// Used to notify the global manager that a local manager is tracking a new guild.
        /// </remarks>
        void TrackCharacterChatChannel(ulong characterId, ChatChannelType type, ulong chatId);

        /// <summary>
        /// Stop tracking an existing chat channel for the supplied character.
        /// </summary>
        /// <remarks>
        /// Used to notify the global manager that a local manager has stopped tracking an existing guild.
        /// </remarks>
        void UntrackCharacterChatChannel(ulong characterId, ChatChannelType type, ulong chatId);

        /// <summary>
        /// Process and delegate a <see cref="ClientChat"/> message from <see cref="IPlayer"/>.
        /// </summary>
        void HandleClientChat(IPlayer player, ClientChat chat);

        /// <summary>
        /// Add the <see cref="IPlayer"/> to the chat channels sessions list for appropriate chat channels.
        /// </summary>
        void JoinDefaultChatChannels(IPlayer player);

        /// <summary>
        /// Remove the <see cref="IPlayer"/> from the chat channels sessions list for appropriate chat channels.
        /// </summary>
        void LeaveDefaultChatChannels(IPlayer player);

        void SendMessage(IGameSession session, string message, string name = "", ChatChannelType type = ChatChannelType.System);
        void SendChatResult(IGameSession session, ChatChannelType type, ulong chatId, ChatResult result);
    }
}