using Microsoft.Extensions.DependencyInjection;
using NexusForever.Database;
using NexusForever.Database.Chat.Model;
using NexusForever.Game.Static.Reputation;
using NexusForever.Game.Static.Social;
using NexusForever.Server.ChatServer.Chat;

namespace NexusForever.Server.ChatServer.Character
{
    public class Character : IWrappedModel<CharacterModel>
    {
        public CharacterModel Model { get; private set; }

        public Identity Identity
        {
            get => new Identity
            {
                Id      = Model.CharacterId,
                RealmId = Model.RealmId
            };
        }

        public IdentityName IdentityName
        {
            get => new IdentityName
            {
                Name      = Model.Name,
                RealmName = Model.RealmName
            };
        }

        public Faction Faction
        {
            get => Model.Faction;
            set => Model.Faction = value;
        }

        public bool IsOnline
        {
            get => Model.IsOnline;
            set => Model.IsOnline = value;
        }

        private readonly Dictionary<ulong, CharacterChatChannel> _channels = [];

        #region Dependency Injection

        private readonly IServiceProvider _serviceProvider;

        public Character(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        #endregion

        /// <summary>
        /// Initialise character with a model.
        /// </summary>
        /// <param name="model">Model to initialise character.</param>
        public void Initialise(CharacterModel model)
        {
            Model = model;

            foreach (CharacterChatChannelModel chatChannelModel in model.Channels)
            {
                var characterChatChannel = _serviceProvider.GetRequiredService<CharacterChatChannel>();
                characterChatChannel.Initialise(chatChannelModel);
                _channels[chatChannelModel.ChatId] = characterChatChannel;
            }
        }

        /// <summary>
        /// Returns the number of chat channels this character is a member of.
        /// </summary>
        public uint GetChannelCount()
        {
            return (uint)_channels.Count;
        }

        public async IAsyncEnumerable<ChatChannel> GetChatChannelsAsync()
        {
            foreach (CharacterChatChannel characterChatChannel in _channels.Values)
            {
                ChatChannel chatChannel = await characterChatChannel.GetChatChannelAsync();
                if (chatChannel != null)
                    yield return chatChannel;
            }
        }


        /// <summary>
        /// Get a chat channel by chat id that this character is a member of.
        /// </summary>
        /// <param name="chatId">The id of the chat channel to return.</param>
        public async Task<ChatChannel> GetChatChannelAsync(ulong chatId)
        {
            if (!_channels.TryGetValue(chatId, out CharacterChatChannel characterChatChannel))
                return null;

            return await characterChatChannel.GetChatChannelAsync();
        }

        /// <summary>
        /// Get a chat channel by type that this character is a member of.
        /// </summary>
        /// <param name="type">The type of the chat channel to return.</param>
        public async Task<ChatChannel> GetChatChannelAsync(ChatChannelType type)
        {
            foreach (CharacterChatChannel characterChatChannel in _channels.Values)
            {
                ChatChannel chatChannel = await characterChatChannel.GetChatChannelAsync();
                if (chatChannel == null)
                    continue;

                if (chatChannel.Type == type)
                    return chatChannel;
            }

            return null;
        }

        /// <summary>
        /// Add a chat channel to the character.
        /// </summary>
        /// <param name="chatChannel">A chat channel which the character is a member of.</param>
        public void AddChatChannel(ChatChannel chatChannel)
        {
            ChatChannelMember member = chatChannel.GetMember(Identity);
            if (member == null)
                return;

            var characterChatChannel = _serviceProvider.GetRequiredService<CharacterChatChannel>();
            characterChatChannel.Initialise();
            characterChatChannel.ChatId = chatChannel.ChatId;

            Model.Channels.Add(characterChatChannel.Model);
            _channels[chatChannel.ChatId] = characterChatChannel;
        }

        /// <summary>
        /// Remove a chat channel from the character.
        /// </summary>
        /// <param name="chatChannel">A chat channel which the character is no longer a member of.</param>
        public void RemoveChatChannel(ChatChannel chatChannel)
        {
            ChatChannelMember member = chatChannel.GetMember(Identity);
            if (member == null)
                return;

            if (!_channels.TryGetValue(chatChannel.ChatId, out CharacterChatChannel characterChatChannel))
                return;

            Model.Channels.Remove(characterChatChannel.Model);
            _channels.Remove(chatChannel.ChatId);
        }
    }
}
