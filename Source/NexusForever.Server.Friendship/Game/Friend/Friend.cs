using NexusForever.Database;
using NexusForever.Database.Friendship.Model;
using NexusForever.Game.Static.Friendship;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Server.Friendship.Game.Character;
using NexusForever.Server.Friendship.Network.Internal;

namespace NexusForever.Server.Friendship.Game.Friend
{
    public class Friend : IWrappedModel<FriendModel>
    {
        public FriendModel Model { get; private set; }

        public ulong Id => Model.Id;

        public Identity InviterIdentity => new()
        {
            Id      = Model.InviterCharacterId,
            RealmId = Model.InviterRealmId
        };

        public Identity InviteeIdentity => new()
        {
            Id      = Model.InviteeCharacterId,
            RealmId = Model.InviteeRealmId
        };

        public string Note
        {
            get => Model.Note;
            private set => Model.Note = value;
        }

        public FriendshipType Type
        {
            get => Model.Type;
            private set => Model.Type = value;
        }

        #region Dependency Injection

        private readonly CharacterManager _characterManager;
        private readonly IInternalMessagePublisher _messagePublisher;

        public Friend(
            CharacterManager characterManager,
            OutboxMessagePublisher messagePublisher)
        {
            _characterManager = characterManager;
            _messagePublisher = messagePublisher;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="FriendAccount"/> with a <see cref="FriendAccountModel"/> model.
        /// </summary>
        /// <param name="model">The model to initialise this <see cref="FriendAccount"/> with.</param>
        /// <exception cref="InvalidOperationException">Throws if <see cref="FriendAccount"/> has already been initialised.</exception>
        public void Initialise(FriendModel model)
        {
            if (Model != null)
                throw new InvalidOperationException("Friend is already initialised.");

            Model = model;
        }

        /// <summary>
        /// Initialise <see cref="Friend"/>.
        /// </summary>
        /// <param name="inviterIdentity">The identity of the inviter character.</param>
        /// <param name="inviteeIdentity">The identity of the invitee character.</param>
        /// <param name="type">The type of friendship.</param>
        /// <exception cref="InvalidOperationException">Throws if <see cref="Friend"/> has already been initialised.</exception>
        public void Initialise(Identity inviterIdentity, Identity inviteeIdentity, FriendshipType type)
        {
            if (Model != null)
                throw new InvalidOperationException("Friend is already initialised.");

            Model = new FriendModel
            {
                InviterCharacterId = inviterIdentity.Id,
                InviterRealmId     = inviterIdentity.RealmId,
                InviteeCharacterId = inviteeIdentity.Id,
                InviteeRealmId     = inviteeIdentity.RealmId,
                Type               = type
            };
        }

        /// <summary>
        /// Get the associated inviter <see cref="Character.Character"/> for this <see cref="Friend"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="Friend"/> only contains the reference to the inviter <see cref="Character.Character"/>.
        /// This method is used to retrieve the full <see cref="Character.Character"/> from the underlying datastore.
        /// </remarks>
        public async Task<Character.Character> GetInviterCharacterAsync()
        {
            return await _characterManager.GetCharacterAsync(InviterIdentity);
        }

        /// <summary>
        /// Get the associated invitee <see cref="Character.Character"/> for this <see cref="Friend"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="Friend"/> only contains the reference to the invitee <see cref="Character.Character"/>.
        /// This method is used to retrieve the full <see cref="Character.Character"/> from the underlying datastore.
        /// </remarks>
        public async Task<Character.Character> GetInviteeCharacterAsync()
        {
            return await _characterManager.GetCharacterAsync(InviteeIdentity);
        }

        /// <summary>
        /// Update the note for this <see cref="Friend"/>.
        /// </summary>
        /// <remarks>
        /// This method also publishes a <see cref="FriendshipNoteUpdatedMessage"/> with details of the note update.
        /// </remarks>
        /// <param name="note">The new note to set.</param>
        public async Task UpdateNoteAsync(string note)
        {
            Note = note;

            await _messagePublisher.PublishAsync(new FriendshipNoteUpdatedMessage
            {
                Friend = await this.ToInternalFriendAsync()
            });
        }

        /// <summary>
        /// Update the <see cref="FriendshipType"/> for this <see cref="Friend"/>.
        /// </summary>
        /// <remarks>
        /// This is used when an already existing friendship is updated to a new type, for example when a friend is added as a rival.
        /// This method also publishes a <see cref="FriendshipTypeUpdatedMessage"/> with details of the type update.
        /// </remarks>
        /// <param name="type">The new type to set.</param>
        public async Task UpdateType(FriendshipType type)
        {
            Type = type;

            await _messagePublisher.PublishAsync(new FriendshipTypeUpdatedMessage
            {
                Friend = await this.ToInternalFriendAsync()
            });
        }
    }
}
