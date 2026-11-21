using NexusForever.Database;
using NexusForever.Database.Friendship.Model;
using NexusForever.Server.Friendship.Game.Friend;

namespace NexusForever.Server.Friendship.Game.Account
{
    public class AccountFriend : IWrappedModel<AccountFriendModel>
    {
        public AccountFriendModel Model { get; private set; }

        public uint AccountId => Model.AccountId;

        public ulong FriendAccountId => Model.FriendAccountId;

        #region Dependency Injection

        private readonly FriendAccountManager _friendManager;

        public AccountFriend(
            FriendAccountManager friendManager)
        {
            _friendManager = friendManager;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="AccountFriend"/> with a <see cref="AccountFriendModel"/> model.
        /// </summary>
        /// <param name="model">The model to initialise this <see cref="AccountFriend"/> with.</param>
        /// <exception cref="InvalidOperationException">Throws if <see cref="AccountFriend"/> has already been initialised.</exception>
        public void Initialise(AccountFriendModel model)
        {
            if (Model != null)
                throw new InvalidOperationException("AccountFriend is already initialised.");

            Model = model;
        }

        /// <summary>
        /// Initialise <see cref="AccountFriend"/> with a reference to a <see cref="FriendAccount"/>.
        /// </summary>
        /// <param name="friendId">Id of the <see cref="FriendAccount"/> to reference.</param>
        /// <exception cref="InvalidOperationException">Throws if <see cref="AccountFriend"/> has already been initialised.</exception>
        public void Initialise(ulong friendId)
        {
            if (Model != null)
                throw new InvalidOperationException("AccountFriend is already initialised.");

            Model = new AccountFriendModel
            {
                FriendAccountId = friendId
            };
        }

        /// <summary>
        /// Get the associated <see cref="FriendAccount"/> for this <see cref="AccountFriend"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="AccountFriend"/> only contains the reference to the <see cref="FriendAccount"/>.
        /// This method is used to retrieve the full <see cref="FriendAccount"/> from the underlying datastore.
        /// </remarks>
        public async Task<FriendAccount> GetFriendAsync()
        {
            return await _friendManager.GetFriendAsync(FriendAccountId);
        }
    }
}
