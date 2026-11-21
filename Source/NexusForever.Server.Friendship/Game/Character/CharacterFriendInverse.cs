using NexusForever.Database;
using NexusForever.Database.Friendship.Model;
using NexusForever.Server.Friendship.Game.Friend;

namespace NexusForever.Server.Friendship.Game.Character
{
    public class CharacterFriendInverse : IWrappedModel<CharacterFriendInverseModel>
    {
        public CharacterFriendInverseModel Model { get; private set; }

        public Identity Identity => new()
        {
            Id      = Model.CharacterId,
            RealmId = Model.RealmId
        };

        public ulong FriendId => Model.FriendId;

        #region Dependency Injection

        private readonly FriendManager _friendManager;

        public CharacterFriendInverse(
            FriendManager friendManager)
        {
            _friendManager = friendManager;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="CharacterFriendInverse"/> with a <see cref="CharacterFriendInverseModel"/> model.
        /// </summary>
        /// <param name="model">The model to initialise this <see cref="CharacterFriendInverse"/> with.</param>
        /// <exception cref="InvalidOperationException">Throws if <see cref="CharacterFriendInverse"/> has already been initialised.</exception>
        public void Initialise(CharacterFriendInverseModel model)
        {
            if (Model != null)
                throw new InvalidOperationException("CharacterFriendInverse is already initialised!");

            Model = model;
        }

        /// <summary>
        /// Initialise <see cref="CharacterFriendInverseModel"/> with a reference to a <see cref="Friend.Friend"/>.
        /// </summary>
        /// <param name="friendId">Id of the <see cref="Friend.Friend"/> to reference.</param>
        /// <exception cref="InvalidOperationException">Throws if <see cref="CharacterFriendInverse"/> has already been initialised.</exception>
        public void Initialise(ulong friendId)
        {
            if (Model != null)
                throw new InvalidOperationException("CharacterFriendInverse is already initialised!");

            Model = new CharacterFriendInverseModel
            {
                FriendId = friendId
            };
        }

        /// <summary>
        /// Get the associated <see cref="Friend.Friend"/> for this <see cref="CharacterFriendInverse"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CharacterFriendInverse"/> only contains the reference to the <see cref="Friend.Friend"/>.
        /// This method is used to retrieve the full <see cref="Friend.Friend"/> from the underlying datastore.
        /// </remarks>
        public async Task<Friend.Friend> GetFriendAsync()
        {
            return await _friendManager.GetFriendAsync(FriendId);
        }
    }
}
