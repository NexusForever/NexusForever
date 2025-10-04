using Microsoft.Extensions.DependencyInjection;
using NexusForever.Database.Chat.Model;
using NexusForever.Database.Chat.Repository;
using NexusForever.Game.Static.Social;

namespace NexusForever.Server.ChatServer.Chat
{
    public class ChatChannelManager
    {
        #region Dependency Injection

        private readonly IServiceProvider _serviceProvider;
        private readonly ChatChannelRepository _repository;

        public ChatChannelManager(
            IServiceProvider serviceProvider,
            ChatChannelRepository repository)
        {
            _serviceProvider = serviceProvider;
            _repository      = repository;
        }

        #endregion

        /// <summary>
        /// Create a new chat channel.
        /// </summary>
        /// <param name="type">Type of the chat channel.</param>
        /// <param name="name">Name for the chat channel.</param>
        /// <param name="password">Password for the chat channel.</param>
        public ChatChannel CreateChatChannel(ChatChannelType type, string name, string password)
        {
            var channel = _serviceProvider.GetRequiredService<ChatChannel>();
            channel.Initialise();
            channel.Type     = type;
            channel.Name     = name;
            channel.Password = string.IsNullOrEmpty(password) ? null : password;
            _repository.AddChatChannel(channel.Model);
            return channel;
        }

        /// <summary>
        /// Remove a chat channel.
        /// </summary>
        /// <param name="chatChannel">Chat channel to remove.</param>
        public void RemoveChatChannel(ChatChannel chatChannel)
        {
            _repository.RemoveChatChannel(chatChannel.Model);
        }

        /// <summary>
        /// Get a chat channel by id.
        /// </summary>
        /// <param name="chatId">Id of the chat channel to return.</param>
        public async Task<ChatChannel> GetChatChannelAsync(ulong chatId)
        {
            ChatChannelModel model = await _repository.GetChatChannelAsync(chatId);
            if (model == null)
                return null;

            var channel = _serviceProvider.GetRequiredService<ChatChannel>();
            channel.Initialise(model);
            return channel;
        }

        /// <summary>
        /// Get all chat channels by reference.
        /// </summary>
        /// <param name="type">Reference type of the chat channel to return.</param>
        /// <param name="value">Reference value of the chat channel to return.</param>
        public async IAsyncEnumerable<ChatChannel> GetChatChannelsAsync(ChatChannelReferenceType type, ulong value)
        {
            foreach (ChatChannelModel model in await _repository.GetChatChannelsAsync(type, value))
            {
                var channel = _serviceProvider.GetRequiredService<ChatChannel>();
                channel.Initialise(model);
                yield return channel;
            }
        }

        /// <summary>
        /// Get chat channel by type and reference.
        /// </summary>
        /// <param name="type">Type of the chat channel to return.</param>
        /// <param name="referenceType">Reference type of the chat channel to return.</param>
        /// <param name="value">Reference value of the chat channel to return.</param>
        public async Task<ChatChannel> GetChatChannelAsync(ChatChannelType type, ChatChannelReferenceType referenceType, ulong value)
        {
            ChatChannelModel model = await _repository.GetChatChannelAsync(type, referenceType, value);
            if (model == null)
                return null;

            var channel = _serviceProvider.GetRequiredService<ChatChannel>();
            channel.Initialise(model);
            return channel;
        }

        /// <summary>
        /// Get chat channel by type and name.
        /// </summary>
        /// <param name="type">Type of the chat channel to return.</param>
        /// <param name="name">Name of the chat channel to return.</param>
        public async Task<ChatChannel> GetChatChannelAsync(ChatChannelType type, string name)
        {
            ChatChannelModel model = await _repository.GetChatChannelAsync(type, name);
            if (model == null)
                return null;

            var channel = _serviceProvider.GetRequiredService<ChatChannel>();
            channel.Initialise(model);
            return channel;
        }
    }
}
