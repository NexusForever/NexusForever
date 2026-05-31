using NexusForever.Database;
using NexusForever.Database.Friendship.Model;
using NexusForever.Server.Friendship.Game.Friend;

namespace NexusForever.Server.Friendship.Game.Account
{
    public class AccountFriendInvite : IWrappedModel<AccountFriendInviteModel>
    {
        public AccountFriendInviteModel Model { get; private set; }

        public uint AccountId => Model.AccountId;

        public ulong FriendAccountInviteId => Model.FriendAccountInviteId;

        #region Dependency Injection

        private readonly FriendAccountManager _friendAccountManager;

        public AccountFriendInvite(
            FriendAccountManager friendAccountManager)
        {
            _friendAccountManager = friendAccountManager;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="AccountFriendInvite"/> with a <see cref="AccountFriendInviteModel"/> model.
        /// </summary>
        /// <param name="model">The model to initialise this <see cref="AccountFriendInvite"/> with.</param>
        /// <exception cref="InvalidOperationException">Throws if <see cref="AccountFriendInvite"/> has already been initialised.</exception>
        public void Initialise(AccountFriendInviteModel model)
        {
            if (Model != null)
                throw new InvalidOperationException("AccountFriendInvite is already initialised.");

            Model = model;
        }

        /// <summary>
        /// Initialise <see cref="AccountFriendInvite"/> with a reference to a <see cref="FriendAccountInvite"/>.
        /// </summary>
        /// <param name="friendInviteId">Id of the <see cref="FriendAccountInvite"/> to reference.</param>
        /// <exception cref="InvalidOperationException">Throws if <see cref="AccountFriendInvite"/> has already been initialised.</exception>
        public void Initialise(ulong friendInviteId)
        {
            if (Model != null)
                throw new InvalidOperationException("AccountFriendInvite is already initialised.");

            Model = new AccountFriendInviteModel()
            {
                FriendAccountInviteId = friendInviteId,
            };
        }

        /// <summary>
        /// Get the associated <see cref="FriendAccountInvite"/> for this <see cref="AccountFriendInvite"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="AccountFriendInvite"/> only contains the reference to the <see cref="FriendAccountInvite"/>.
        /// This method is used to retrieve the full <see cref="FriendAccountInvite"/> from the underlying datastore.
        /// </remarks>
        public async Task<FriendAccountInvite> GetFriendInviteAsync()
        {
            return await _friendAccountManager.GetFriendInviteAsync(FriendAccountInviteId);
        }
    }
}
