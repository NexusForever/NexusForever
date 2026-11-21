using NexusForever.Database;
using NexusForever.Database.Friendship.Model;
using NexusForever.Server.Friendship.Game.Friend;

namespace NexusForever.Server.Friendship.Game.Account
{
    public class AccountFriendInverse : IWrappedModel<AccountFriendInverseModel>
    {
        public AccountFriendInverseModel Model { get; private set; }

        public uint AccountId => Model.AccountId;

        public ulong FriendAccountId => Model.FriendAccountId;

        #region Dependency Injection

        private readonly FriendAccountManager _friendManager;

        public AccountFriendInverse(
            FriendAccountManager friendManager)
        {
            _friendManager = friendManager;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="AccountFriendInverse"/> with a <see cref="AccountFriendInverseModel"/> model.
        /// </summary>
        /// <param name="model">The model to initialise this <see cref="AccountFriendInverse"/> with.</param>
        /// <exception cref="InvalidOperationException">Throws if <see cref="AccountFriendInverse"/> has already been initialised.</exception>
        public void Initialise(AccountFriendInverseModel model)
        {
            if (Model != null)
                throw new InvalidOperationException("AccountFriendInverse is already initialised.");

            Model = model;
        }

        /// <summary>
        /// Initialise <see cref="AccountFriendInverse"/> with a reference to a <see cref="FriendAccount"/>.
        /// </summary>
        /// <param name="friendId">Id of the <see cref="FriendAccount"/> to reference.</param>
        /// <exception cref="InvalidOperationException">Throws if <see cref="AccountFriendInverse"/> has already been initialised.</exception>
        public void Initialise(ulong friendId)
        {
            if (Model != null)
                throw new InvalidOperationException("AccountFriendInverse is already initialised.");

            Model = new AccountFriendInverseModel
            {
                FriendAccountId = friendId
            };
        }

        /// <summary>
        /// Get the associated <see cref="FriendAccount"/> for this <see cref="AccountFriendInverse"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="AccountFriendInverse"/> only contains the reference to the <see cref="FriendAccount"/>.
        /// This method is used to retrieve the full <see cref="FriendAccount"/> from the underlying datastore.
        /// </remarks>
        public async Task<FriendAccount> GetFriendAsync()
        {
            return await _friendManager.GetFriendAsync(Model.FriendAccountId);
        }
    }
}
