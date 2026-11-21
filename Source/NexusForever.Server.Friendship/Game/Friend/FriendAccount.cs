using NexusForever.Database;
using NexusForever.Database.Friendship.Model;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Server.Friendship.Game.Account;

namespace NexusForever.Server.Friendship.Game.Friend
{
    public class FriendAccount : IWrappedModel<FriendAccountModel>
    {
        public FriendAccountModel Model { get; private set; }

        public ulong Id => Model.Id;

        public uint InviterAccountId => Model.InviterAccountId;

        public uint InviteeAccountId => Model.InviteeAccountId;

        public string Note
        {
            get => Model.Note;
            private set => Model.Note = value;
        }

        #region Dependency Injection

        private readonly AccountManager _accountManager;
        private readonly IInternalMessagePublisher _messagePublisher;

        public FriendAccount(
            AccountManager accountManager,
            IInternalMessagePublisher messagePublisher)
        {
            _accountManager   = accountManager;
            _messagePublisher = messagePublisher;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="FriendAccount"/> with a <see cref="FriendAccountModel"/> model.
        /// </summary>
        /// <param name="model">The model to initialise this <see cref="FriendAccount"/> with.</param>
        /// <exception cref="InvalidOperationException">Throws if <see cref="FriendAccount"/> has already been initialised.</exception>
        public void Initialise(FriendAccountModel model)
        {
            if (Model != null)
                throw new InvalidOperationException("FriendAccount is already initialised.");

            Model = model;
        }

        /// <summary>
        /// Initialise <see cref="FriendAccount"/>.
        /// </summary>
        /// <param name="inviterAccountId">The id of the inviter account.</param>
        /// <param name="inviteeAccountId">The id of the invitee account.</param>
        /// <exception cref="InvalidOperationException">Throws if <see cref="FriendAccount"/> has already been initialised.</exception>
        public void Initialise(uint inviterAccountId, uint inviteeAccountId)
        {
            if (Model != null)
                throw new InvalidOperationException("FriendAccount is already initialised.");

            Model = new FriendAccountModel
            {
                InviterAccountId = inviterAccountId,
                InviteeAccountId = inviteeAccountId
            };
        }

        /// <summary>
        /// Get the associated inviter <see cref="Account.Account"/> for this <see cref="FriendAccount"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="FriendAccount"/> only contains the reference to the inviter <see cref="Account.Account"/>.
        /// This method is used to retrieve the full <see cref="Account.Account"/> from the underlying datastore.
        /// </remarks>
        public async Task<Account.Account> GetInviterAccountAsync()
        {
            return await _accountManager.GetAccountAsync(InviterAccountId);
        }

        /// <summary>
        /// Get the associated invitee <see cref="Account.Account"/> for this <see cref="FriendAccount"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="FriendAccount"/> only contains the reference to the invitee <see cref="Account.Account"/>.
        /// This method is used to retrieve the full <see cref="Account.Account"/> from the underlying datastore.
        /// </remarks>
        public async Task<Account.Account> GetInviteeAccountAsync()
        {
            return await _accountManager.GetAccountAsync(InviteeAccountId);
        }

        /// <summary>
        /// Update the note for this <see cref="FriendAccount"/>.
        /// </summary>
        /// <remarks>
        /// This method also publishes a <see cref="FriendshipAccountNoteUpdatedMessage"/> with details of the note update.
        /// </remarks>
        /// <param name="note">The new note to set.</param>
        public async Task UpdateNoteAsync(string note)
        {
            Note = note;

            await _messagePublisher.PublishAsync(new FriendshipAccountNoteUpdatedMessage
            {
                FriendAccount = await this.ToInternalFriendAsync()
            });
        }
    }
}
