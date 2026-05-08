using NexusForever.Database;
using NexusForever.Database.Friendship.Model;
using NexusForever.Server.Friendship.Game.Friend;

namespace NexusForever.Server.Friendship.Game.Account
{
    public class AccountFriendInvitePending : IWrappedModel<AccountFriendInvitePendingModel>
    {
        public AccountFriendInvitePendingModel Model { get; private set; }

        public uint AccountId => Model.AccountId;

        public ulong FriendAccountInviteId => Model.FriendAccountInviteId;

        #region Dependency Injection

        private readonly FriendAccountManager _friendManager;

        public AccountFriendInvitePending(
            FriendAccountManager friendManager)
        {
            _friendManager = friendManager;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="AccountFriendInvitePending"/> with a <see cref="AccountFriendInvitePendingModel"/> model.
        /// </summary>
        /// <param name="model">The model to initialise this <see cref="AccountFriendInvitePending"/> with.</param>
        /// <exception cref="InvalidOperationException">Throws if <see cref="AccountFriendInvitePending"/> has already been initialised.</exception>
        public void Initialise(AccountFriendInvitePendingModel model)
        {
            if (Model != null)
                throw new InvalidOperationException("AccountFriendInvitePending already initialised!");

            Model = model;
        }

        /// <summary>
        /// Initialise <see cref="AccountFriendInvitePending"/> with a reference to a <see cref="FriendAccountInvite"/>.
        /// </summary>
        /// <param name="friendInviteId">Id of the <see cref="FriendAccountInvite"/> to reference.</param>
        /// <exception cref="InvalidOperationException">Throws if <see cref="AccountFriendInvitePending"/> has already been initialised.</exception>
        public void Initialise(ulong friendInviteId)
        {
            if (Model != null)
                throw new InvalidOperationException("AccountFriendInvitePending already initialised!");

            Model = new AccountFriendInvitePendingModel
            {
                FriendAccountInviteId = friendInviteId
            };
        }

        /// <summary>
        /// Get the associated <see cref="FriendAccountInvite"/> for this <see cref="AccountFriendInvitePending"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="AccountFriendInvitePending"/> only contains the reference to the <see cref="FriendAccountInvite"/>.
        /// This method is used to retrieve the full <see cref="FriendAccountInvite"/> from the underlying datastore.
        /// </remarks>
        public async Task<FriendAccountInvite> GetFriendInviteAsync()
        {
            return await _friendManager.GetFriendInviteAsync(FriendAccountInviteId);
        }
    }
}
