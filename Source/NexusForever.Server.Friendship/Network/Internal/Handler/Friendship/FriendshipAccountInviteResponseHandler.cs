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
    public class FriendshipAccountInviteResponseHandler : IHandleMessages<FriendshipAccountInviteResponseMessage>
    {
        #region Dependency Injection

        private readonly FriendshipContext _context;
        private readonly IInternalMessagePublisher _messagePublisher;

        private readonly AccountManager _accountManager;
        private readonly FriendAccountManager _friendManager;
        private readonly FriendAccountFactory _friendFactory;
        private readonly FriendshipResultPublisher _friendshipResultPublisher;

        public FriendshipAccountInviteResponseHandler(
            FriendshipContext context,
            OutboxMessagePublisher messagePublisher,
            AccountManager accountManager,
            FriendAccountManager friendManager,
            FriendAccountFactory friendFactory,
            FriendshipResultPublisher friendshipResultPublisher)
        {
            _context                   = context;
            _messagePublisher          = messagePublisher;
            _accountManager            = accountManager;
            _friendManager             = friendManager;
            _friendFactory             = friendFactory;
            _friendshipResultPublisher = friendshipResultPublisher;
        }

        #endregion

        public async Task Handle(FriendshipAccountInviteResponseMessage message)
        {
            IExecutionStrategy strategy = _context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();

                Task<FriendshipResult?> task = RespondAsync(message);
                await _friendshipResultPublisher.PublishResultAsync(_messagePublisher, message.Invitee, task);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            });
        }

        private async Task<FriendshipResult?> RespondAsync(FriendshipAccountInviteResponseMessage message)
        {
            Account invitee = await _accountManager.GetAccountAsync(message.AccountId);
            if (invitee == null)
                return FriendshipResult.PlayerNotFound;

            FriendAccountInvite invite = await invitee.GetFriendInviteAsync(message.InviteId);
            if (invite == null)
                return FriendshipResult.RequestNotFound;

            if (invite.HasExpired())
                return FriendshipResult.RequestTimedOut;

            Account inviter = await invite.GetInviterAccountAsync();
            if (inviter == null)
                return FriendshipResult.PlayerNotFound;

            if (message.Response)
            {
                _ = await _friendFactory.CreateFriendAsync(inviter, invitee);
                _ = await _friendFactory.CreateFriendAsync(invitee, inviter);
            }

            await invitee.RemoveFriendInviteAsync(invite);
            inviter.RemoveFriendInvitePending(invite);

            _friendManager.RemoveFriendInvite(invite);

            return null;
        }
    }
}
