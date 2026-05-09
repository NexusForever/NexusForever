using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NexusForever.Database.Friendship;
using NexusForever.Game.Static.Friendship;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Server.Friendship.Game.Account;
using NexusForever.Server.Friendship.Game.Friend;
using Rebus.Handlers;

namespace NexusForever.Server.Friendship.Network.Internal.Handler.Friendship
{
    public class FriendshipAccountEmailInviteRequestHandler : IHandleMessages<FriendshipAccountEmailInviteRequestMessage>
    {
        #region Dependency Injection

        private readonly FriendshipContext _context;
        private readonly IInternalMessagePublisher _messagePublisher;

        private readonly AccountManager _accountManager;
        private readonly FriendAccountInviteValidator _inviteValidator;
        private readonly FriendAccountInviteFactory _inviteFactory;
        private readonly FriendshipResultPublisher _friendshipResultPublisher;

        public FriendshipAccountEmailInviteRequestHandler(
            FriendshipContext context,
            OutboxMessagePublisher messagePublisher,
            AccountManager accountManager,
            FriendAccountInviteValidator inviteValidator,
            FriendAccountInviteFactory inviteFactory,
            FriendshipResultPublisher friendshipResultPublisher)
        {
            _context                   = context;
            _messagePublisher          = messagePublisher;
            _accountManager            = accountManager;
            _inviteValidator           = inviteValidator;
            _inviteFactory             = inviteFactory;
            _friendshipResultPublisher = friendshipResultPublisher;
        }

        #endregion

        public async Task Handle(FriendshipAccountEmailInviteRequestMessage message)
        {
            IExecutionStrategy strategy = _context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();

                Task<FriendshipResult?> task = InviteAsync(message);
                await _friendshipResultPublisher.PublishResultAsync(_messagePublisher, message.Inviter, task);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            });
        }

        private async Task<FriendshipResult?> InviteAsync(FriendshipAccountEmailInviteRequestMessage message)
        {
            Account inviterAccount = await _accountManager.GetAccountAsync(message.InviterAccountId);
            if (inviterAccount == null)
                return FriendshipResult.PlayerNotFound;

            Account inviteeAccount = await _accountManager.GetAccountByEmailAsync(message.Email);
            if (inviteeAccount == null)
                return FriendshipResult.PlayerNotFound;

            FriendshipResult? result = await _inviteValidator.ValidateAsync(inviterAccount, inviteeAccount, message.Note);
            if (result != null)
                return result.Value;

            _ = await _inviteFactory.CreateFriendInviteAsync(inviterAccount, inviteeAccount, message.Note);

            return null;
        }
    }
}
