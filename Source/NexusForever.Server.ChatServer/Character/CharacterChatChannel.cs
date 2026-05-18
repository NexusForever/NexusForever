using NexusForever.Database;
using NexusForever.Database.Chat.Model;
using NexusForever.Server.ChatServer.Chat;

namespace NexusForever.Server.ChatServer.Character
{
    public class CharacterChatChannel : IWrappedModel<CharacterChatChannelModel>
    {
        public CharacterChatChannelModel Model { get; private set; }

        public ulong ChatId
        {
            get => Model.ChatId;
            set => Model.ChatId = value;
        }

        #region Dependency Injection

        private readonly ChatChannelManager _chatChannelManager;

        public CharacterChatChannel(
            ChatChannelManager chatChannelManager)
        {
            _chatChannelManager = chatChannelManager;
        }

        #endregion

        /// <summary>
        /// Initialise a new character chat channel.
        /// </summary>
        public void Initialise()
        {
            if (Model != null)
                throw new InvalidOperationException("CharacterChatChannel has already been initialised.");

            Model = new CharacterChatChannelModel();
        }

        /// <summary>
        /// Initialise character chat channel with model.
        /// </summary>
        /// <param name="model">Model to initialise the character chat channel.</param>
        public void Initialise(CharacterChatChannelModel model)
        {
            if (Model != null)
                throw new InvalidOperationException("CharacterChatChannel has already been initialised.");

            Model = model;
        }

        /// <summary>
        /// Get the chat channel associated with this character chat channel.
        /// </summary>
        public async Task<ChatChannel> GetChatChannelAsync()
        {
            return await _chatChannelManager.GetChatChannelAsync(ChatId);
        }
    }
}
