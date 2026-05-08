using NexusForever.Database;
using NexusForever.Database.Friendship.Model;
using NexusForever.Server.Friendship.Game.Friend;

namespace NexusForever.Server.Friendship.Game.Character
{
    public class CharacterFriendInvite : IWrappedModel<CharacterFriendInviteModel>
    {
        public CharacterFriendInviteModel Model { get; private set; }

        public ulong FriendInviteId => Model.FriendInviteId;

        #region Dependency Injection

        private readonly FriendManager _friendManager;

        public CharacterFriendInvite(
            FriendManager friendManager)
        {
            _friendManager = friendManager;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="CharacterFriendInvite"/> with a <see cref="CharacterFriendInviteModel"/> model.
        /// </summary>
        /// <param name="model">The model to initialise this <see cref="CharacterFriendInvite"/> with.</param>
        /// <exception cref="InvalidOperationException">Throws if <see cref="CharacterFriendInvite"/> has already been initialised.</exception>
        public void Initialise(CharacterFriendInviteModel model)
        {
            if (Model != null)
                throw new InvalidOperationException("CharacterFriendInvite is already initialised.");

            Model = model;
        }

        /// <summary>
        /// Initialise <see cref="CharacterFriendInvite"/> with a reference to a <see cref="FriendInvite"/>.
        /// </summary>
        /// <param name="friendInviteId">Id of the <see cref="FriendInvite"/> to reference.</param>
        /// <exception cref="InvalidOperationException">Throws if <see cref="CharacterFriendInvite"/> has already been initialised.</exception>
        public void Initialise(ulong friendInviteId)
        {
            if (Model != null)
                throw new InvalidOperationException("CharacterFriendInvite is already initialised.");

            Model = new CharacterFriendInviteModel()
            {
                FriendInviteId = friendInviteId
            };
        }

        /// <summary>
        /// Get the associated <see cref="FriendInvite"/> for this <see cref="CharacterFriendInvite"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CharacterFriendInvite"/> only contains the reference to the <see cref="FriendInvite"/>.
        /// This method is used to retrieve the full <see cref="FriendInvite"/> from the underlying datastore.
        /// </remarks>
        public async Task<FriendInvite> GetFriendInviteAsync()
        {
            return await _friendManager.GetFriendInviteAsync(FriendInviteId);
        }
    }
}
