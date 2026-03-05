using NexusForever.Database;
using NexusForever.Database.Chat.Model;
using NexusForever.Game.Static.Chat;
using NexusForever.Server.ChatServer.Character;

namespace NexusForever.Server.ChatServer.Chat
{
    public class ChatChannelMember : IWrappedModel<ChatChannelMemberModel>
    {
        public ChatChannelMemberModel Model { get; private set; }

        public ulong ChatId
        {
            get => Model.ChatId;
            set => Model.ChatId = value;
        }

        public Identity Identity
        {
            get => new Identity
            {
                Id      = Model.CharacterId,
                RealmId = Model.RealmId
            };
            set
            {
                Model.CharacterId = value.Id;
                Model.RealmId     = value.RealmId;
            }
        }

        public ChatChannelMemberFlags Flags
        {
            get => Model.Flags;
            set => Model.Flags = value;
        }

        #region Dependency Injection

        private readonly CharacterManager _characterManager;

        public ChatChannelMember(
            CharacterManager characterManager)
        {
            _characterManager = characterManager;
        }

        #endregion

        /// <summary>
        /// Initialise a new chat channel member.
        /// </summary>
        public void Initialise()
        {
            if (Model != null)
                throw new InvalidOperationException("ChatChannelMember is already initialised.");

            Model = new ChatChannelMemberModel();
        }

        /// <summary>
        /// Initialise a chat channel member with an existing model.
        /// </summary>
        /// <param name="model">Model to initialise the chat channel member.</param>
        public void Initialise(ChatChannelMemberModel model)
        {
            if (Model != null)
                throw new InvalidOperationException("ChatChannelMember is already initialised.");

            Model = model;
        }

        /// <summary>
        /// Get the character associated with this chat channel member.
        /// </summary>
        public async Task<Character.Character> GetCharacterAsync()
        {
            return await _characterManager.GetCharacterRemoteAsync(Identity);
        }
    }
}
