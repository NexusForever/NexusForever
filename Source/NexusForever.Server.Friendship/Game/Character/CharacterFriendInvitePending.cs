using NexusForever.Database;
using NexusForever.Database.Friendship.Model;
using NexusForever.Server.Friendship.Game.Friend;

namespace NexusForever.Server.Friendship.Game.Character
{
    public class CharacterFriendInvitePending : IWrappedModel<CharacterFriendInvitePendingModel>
    {
        public CharacterFriendInvitePendingModel Model { get; private set; }

        public Identity Identity => new()
        {
            Id      = Model.CharacterId,
            RealmId = Model.RealmId
        };

        public ulong FriendInviteId => Model.FriendInviteId;

        #region Dependency Injection

        private readonly FriendManager _friendManager;

        public CharacterFriendInvitePending(
            FriendManager friendManager)
        {
            _friendManager = friendManager;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="CharacterFriendInvitePending"/> with a <see cref="CharacterFriendInvitePendingModel"/> model.
        /// </summary>
        /// <param name="model">The model to initialise this <see cref="CharacterFriendInvitePending"/> with.</param>
        /// <exception cref="InvalidOperationException">Throws if <see cref="CharacterFriendInvitePending"/> has already been initialised.</exception>
        public void Initialise(CharacterFriendInvitePendingModel model)
        {
            Model = model;
        }

        /// <summary>
        /// Initialise <see cref="CharacterFriendInvitePending"/> with a reference to a <see cref="FriendInvite"/>.
        /// </summary>
        /// <param name="friendInviteId">Id of the <see cref="FriendInvite"/> to reference.</param>
        /// <exception cref="InvalidOperationException">Throws if <see cref="CharacterFriendInvitePending"/> has already been initialised.</exception>
        public void Initialise(ulong friendInviteId)
        {
            Model = new CharacterFriendInvitePendingModel
            {
                FriendInviteId = friendInviteId
            };
        }

        /// <summary>
        /// Get the associated <see cref="FriendInvite"/> for this <see cref="CharacterFriendInvitePending"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CharacterFriendInvitePending"/> only contains the reference to the <see cref="FriendInvite"/>.
        /// This method is used to retrieve the full <see cref="FriendInvite"/> from the underlying datastore.
        /// </remarks>
        public async Task<FriendInvite> GetFriendInviteAsync()
        {
            return await _friendManager.GetFriendInviteAsync(FriendInviteId);
        }
    }
}
