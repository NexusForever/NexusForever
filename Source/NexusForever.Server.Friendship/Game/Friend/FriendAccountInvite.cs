using Microsoft.Extensions.Options;
using NexusForever.Database;
using NexusForever.Database.Friendship.Model;
using NexusForever.Server.Friendship.Configuration;
using NexusForever.Server.Friendship.Game.Account;

namespace NexusForever.Server.Friendship.Game.Friend
{
    public class FriendAccountInvite : IWrappedModel<FriendAccountInviteModel>
    {
        public FriendAccountInviteModel Model { get; private set; }

        public ulong Id => Model.Id;

        public uint InviteeAccountId => Model.InviteeAccountId;

        public uint InviterAccountId => Model.InviterAccountId;

        public bool Seen
        {
            get => Model.Seen;
            set => Model.Seen = value;
        }

        public string Note
        {
            get => Model.Note;
            private set => Model.Note = value;
        }

        public DateTime Expiration => Model.Expiration;

        #region Dependency Injection

        private readonly InviteOptions _inviteOptions;
        private readonly AccountManager _accountManager;

        public FriendAccountInvite(
            IOptions<InviteOptions> inviteOptions,
            AccountManager accountManager)
        {
            _inviteOptions  = inviteOptions.Value;
            _accountManager = accountManager;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="FriendAccountInvite"/> with a <see cref="FriendAccountInviteModel"/> model.
        /// </summary>
        /// <param name="model">The model to initialise this <see cref="FriendAccountInvite"/> with.</param>
        /// <exception cref="InvalidOperationException">Throws if <see cref="FriendAccountInvite"/> has already been initialised.</exception>
        public void Initialise(FriendAccountInviteModel model)
        {
            if (Model != null)
                throw new InvalidOperationException("FriendAccountInvite is already initialised.");

            Model = model;
        }

        /// <summary>
        /// Initialise <see cref="FriendAccountInvite"/>.
        /// </summary>
        /// <param name="inviterAccountId">The id of the inviter account.</param>
        /// <param name="inviteeAccountId">The id of the invitee account.</param>
        /// <param name="note">Optional note for the friend invite.</param>
        /// <exception cref="InvalidOperationException">Throws if <see cref="FriendAccountInvite"/> has already been initialised.</exception>
        public void Initialise(uint inviterAccountId, uint inviteeAccountId, string note)
        {
            if (Model != null)
                throw new InvalidOperationException("FriendAccountInvite is already initialised.");

            Model = new FriendAccountInviteModel
            {
                InviteeAccountId = inviteeAccountId,
                InviterAccountId = inviterAccountId,
                Note             = note,
                Expiration       = DateTime.UtcNow.AddMinutes(_inviteOptions.AccountInviteLifetimeMinutes)
            };
        }

        /// <summary>
        /// Get the associated invitee <see cref="Account.Account"/> for this <see cref="FriendAccountInvite"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="FriendAccountInvite"/> only contains the reference to the invitee <see cref="Account.Account"/>.
        /// This method is used to retrieve the full <see cref="Account.Account"/> from the underlying datastore.
        /// </remarks>
        public async Task<Account.Account> GetInviteeAccountAsync()
        {
            return await _accountManager.GetAccountAsync(InviteeAccountId);
        }

        /// <summary>
        /// Get the associated inviter <see cref="Account.Account"/> for this <see cref="FriendAccountInvite"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="FriendAccountInvite"/> only contains the reference to the inviter <see cref="Account.Account"/>.
        /// This method is used to retrieve the full <see cref="Account.Account"/> from the underlying datastore.
        /// </remarks>
        public async Task<Account.Account> GetInviterAccountAsync()
        {
            return await _accountManager.GetAccountAsync(InviterAccountId);
        }

        /// <summary>
        /// Returns whether the <see cref="FriendAccountInvite"/> has expired.
        /// </summary>
        public bool HasExpired()
        {
            return DateTime.UtcNow > Expiration;
        }
    }
}
