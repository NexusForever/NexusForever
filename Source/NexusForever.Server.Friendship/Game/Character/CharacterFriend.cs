using NexusForever.Database;
using NexusForever.Database.Friendship.Model;
using NexusForever.Server.Friendship.Game.Friend;

namespace NexusForever.Server.Friendship.Game.Character
{
    public class CharacterFriend : IWrappedModel<CharacterFriendModel>
    {
        public CharacterFriendModel Model { get; private set; }

        public Identity Identity
        {
            get => new()
            {
                Id      = Model.CharacterId,
                RealmId = Model.RealmId
            };
        }

        public ulong FriendId => Model.FriendId;

        #region Dependency Injection

        private readonly FriendManager _friendManager;

        public CharacterFriend(
            FriendManager friendManager)
        {
            _friendManager = friendManager;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="CharacterFriend"/> with a <see cref="CharacterFriendModel"/> model.
        /// </summary>
        /// <param name="model">The model to initialise this <see cref="CharacterFriend"/> with.</param>
        /// <exception cref="InvalidOperationException">Throws if <see cref="CharacterFriend"/> has already been initialised.</exception>
        public void Initialise(CharacterFriendModel model)
        {
            if (Model != null)
                throw new InvalidOperationException("CharacterFriend is already initialised.");

            Model = model;
        }

        /// <summary>
        /// Initialise <see cref="CharacterFriend"/> with a reference to a <see cref="Friend.Friend"/>.
        /// </summary>
        /// <param name="id">Id of the <see cref="Friend.Friend"/> to reference.</param>
        /// <exception cref="InvalidOperationException">Throws if <see cref="CharacterFriend"/> has already been initialised.</exception>
        public void Initialise(ulong friendId)
        {
            if (Model != null)
                throw new InvalidOperationException("CharacterFriend is already initialised.");

            Model = new CharacterFriendModel
            {
                FriendId = friendId
            };
        }

        /// <summary>
        /// Get the associated <see cref="Friend.Friend"/> for this <see cref="CharacterFriend"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CharacterFriend"/> only contains the reference to the <see cref="Friend.Friend"/>.
        /// This method is used to retrieve the full <see cref="Friend.Friend"/> from the underlying datastore.
        /// </remarks>
        public async Task<Friend.Friend> GetFriendAsync()
        {
            return await _friendManager.GetFriendAsync(FriendId);
        }
    }
}
