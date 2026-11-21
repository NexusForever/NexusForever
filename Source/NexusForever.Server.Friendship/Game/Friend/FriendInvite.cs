using Microsoft.Extensions.Options;
using NexusForever.Database;
using NexusForever.Database.Friendship.Model;
using NexusForever.Server.Friendship.Configuration;
using NexusForever.Server.Friendship.Game.Character;

namespace NexusForever.Server.Friendship.Game.Friend
{
    public class FriendInvite : IWrappedModel<FriendInviteModel>
    {
        public FriendInviteModel Model { get; private set; }

        public ulong Id => Model.Id;

        public Identity InviteeCharacterIdentity => new()
        {
            Id      = Model.InviteeCharacterId,
            RealmId = Model.InviteeRealmId
        };

        public Identity InviterCharacterIdentity => new()
        {
            Id      = Model.InviterCharacterId,
            RealmId = Model.InviterRealmId
        };

        public bool Seen
        {
            get => Model.Seen;
            set => Model.Seen = value;
        }

        public string Note => Model.Note;

        public DateTime Expiration => Model.Expiration;

        #region Dependency Injection

        private readonly InviteOptions _inviteOptions;
        private readonly CharacterManager _characterManager;

        public FriendInvite(
            IOptions<InviteOptions> inviteOptions,
            CharacterManager characterManager)
        {
            _inviteOptions    = inviteOptions.Value;
            _characterManager = characterManager;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="FriendInvite"/> with a <see cref="FriendInviteModel"/> model.
        /// </summary>
        /// <param name="model">The model to initialise this <see cref="FriendInvite"/> with.</param>
        /// <exception cref="InvalidOperationException">Throws if <see cref="FriendInvite"/> has already been initialised.</exception>
        public void Initialise(FriendInviteModel model)
        {
            if (Model != null)
                throw new InvalidOperationException("FriendInvite is already initialised.");

            Model = model;
        }

        /// <summary>
        /// Initialise <see cref="FriendInvite"/>.
        /// </summary>
        /// <param name="inviter">The identity of the inviter character.</param>
        /// <param name="invitee">The identity of the invitee character.</param>
        /// <param name="note">Optional note for the friend invite.</param>
        /// <exception cref="InvalidOperationException">Throws if <see cref="FriendInvite"/> has already been initialised.</exception>
        public void Initialise(Identity inviter, Identity invitee, string note)
        {
            if (Model != null)
                throw new InvalidOperationException("FriendInvite is already initialised.");

            Model = new FriendInviteModel
            {
                InviteeCharacterId = invitee.Id,
                InviteeRealmId     = invitee.RealmId,
                InviterCharacterId = inviter.Id,
                InviterRealmId     = inviter.RealmId,
                Note               = note,
                Expiration         = DateTime.UtcNow.AddMinutes(_inviteOptions.InviteLifetimeMinutes)
            };
        }

        /// <summary>
        /// Get the associated invitee <see cref="Character.Character"/> for this <see cref="FriendInvite"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="FriendInvite"/> only contains the reference to the invitee <see cref="Character.Character"/>.
        /// This method is used to retrieve the full <see cref="Character.Character"/> from the underlying datastore.
        /// </remarks>
        public async Task<Character.Character> GetInviteeCharacterAsync()
        {
            return await _characterManager.GetCharacterAsync(InviteeCharacterIdentity);
        }

        /// <summary>
        /// Get the associated inviter <see cref="Character.Character"/> for this <see cref="FriendInvite"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="FriendInvite"/> only contains the reference to the inviter <see cref="Character.Character"/>.
        /// This method is used to retrieve the full <see cref="Character.Character"/> from the underlying datastore.
        /// </remarks>
        public async Task<Character.Character> GetInviterCharacterAsync()
        {
            return await _characterManager.GetCharacterAsync(InviterCharacterIdentity);
        }

        /// <summary>
        /// Returns whether the <see cref="FriendInvite"/> has expired.
        /// </summary>
        public bool HasExpired()
        {
            return DateTime.UtcNow > Expiration;
        }
    }
}
