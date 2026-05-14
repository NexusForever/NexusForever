using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NexusForever.Database.Friendship;
using NexusForever.Game.Static.Friendship;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Server.Friendship.Game.Character;
using NexusForever.Server.Friendship.Game.Friend;
using Rebus.Handlers;

namespace NexusForever.Server.Friendship.Network.Internal.Handler.Friendship
{
    public class FriendshipIdentityInviteRequestHandler : IHandleMessages<FriendshipIdentityInviteRequestMessage>
    {
        #region Dependency Injection

        private readonly FriendshipContext _context;
        private readonly IInternalMessagePublisher _messagePublisher;

        private readonly CharacterManager _characterManager;
        private readonly FriendInviteValidator _inviteValidator;
        private readonly FriendRequestHandler _friendRequestHandler;
        private readonly FriendshipResultPublisher _friendshipResultPublisher;

        public FriendshipIdentityInviteRequestHandler(
            FriendshipContext context,
            IInternalMessagePublisher messagePublisher,
            CharacterManager characterManager,
            FriendInviteValidator inviteValidator,
            FriendRequestHandler friendRequestHandler,
            FriendshipResultPublisher friendshipResultPublisher)
        {
            _context                   = context;
            _messagePublisher          = messagePublisher;
            _characterManager          = characterManager;
            _inviteValidator           = inviteValidator;
            _friendRequestHandler      = friendRequestHandler;
            _friendshipResultPublisher = friendshipResultPublisher;
        }

        #endregion

        public async Task Handle(FriendshipIdentityInviteRequestMessage message)
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

        private async Task<FriendshipResult?> InviteAsync(FriendshipIdentityInviteRequestMessage message)
        {
            Character inviter = await _characterManager.GetCharacterRemoteAsync(message.Inviter.ToFriendshipIdentity());
            if (inviter == null)
                return FriendshipResult.PlayerNotFound;

            Character invitee = await _characterManager.GetCharacterRemoteAsync(message.Invitee.ToFriendshipIdentity());
            if (invitee == null)
                return FriendshipResult.PlayerNotFound;

            FriendshipResult? result = await _inviteValidator.ValidatorAsync(inviter, invitee, message.Type, message.Note);
            if (result != null)
                return result.Value;

            if (await inviter.GetFriendInvitePendingAsync(invitee.Identity) != null)
                return FriendshipResult.PlayerQueuedRequests;

            return await _friendRequestHandler.HandleRequestAsync(inviter, invitee, message.Type, message.Note);
        }
    }
}
