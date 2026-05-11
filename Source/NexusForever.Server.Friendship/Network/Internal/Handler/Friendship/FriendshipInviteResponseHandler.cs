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
    public class FriendshipInviteResponseHandler : IHandleMessages<FriendshipInviteResponseMessage>
    {
        #region Dependency Injection

        private readonly FriendshipContext _context;
        private readonly IInternalMessagePublisher _messagePublisher;

        private readonly CharacterManager _characterManager;
        private readonly FriendManager _friendManager;
        private readonly FriendFactory _friendFactory;
        private readonly FriendInviteValidator _inviteValidator;
        private readonly FriendshipResultPublisher _friendshipResultPublisher;

        public FriendshipInviteResponseHandler(
            FriendshipContext context,
            OutboxMessagePublisher messagePublisher,
            CharacterManager characterManager,
            FriendManager friendManager,
            FriendFactory friendFactory,
            FriendInviteValidator inviteValidator,
            FriendshipResultPublisher friendshipResultPublisher)
        {
            _context                   = context;
            _messagePublisher          = messagePublisher;
            _characterManager          = characterManager;
            _friendManager             = friendManager;
            _friendFactory             = friendFactory;
            _inviteValidator           = inviteValidator;
            _friendshipResultPublisher = friendshipResultPublisher;
        }

        #endregion

        public async Task Handle(FriendshipInviteResponseMessage message)
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

        private async Task<FriendshipResult?> RespondAsync(FriendshipInviteResponseMessage message)
        {
            Character invitee = await _characterManager.GetCharacterAsync(message.Invitee.ToFriendshipIdentity());
            if (invitee == null)
                return FriendshipResult.PlayerNotFound;

            FriendInvite invite = await invitee.GetFriendInviteAsync(message.InviteId);
            if (invite == null)
                return FriendshipResult.RequestNotFound;

            if (invite.HasExpired())
                return FriendshipResult.RequestTimedOut;

            Character inviter = await invite.GetInviterCharacterAsync();
            if (invite == null)
                return FriendshipResult.PlayerNotFound;

            // do a final validation before accepting the invite in case something has changed since the invite was sent
            if (await _inviteValidator.ValidatorAsync(inviter, invitee, FriendshipType.Friend, null) != null)
                return FriendshipResult.UnableToProcess;

            switch (message.Response)
            {
                case FriendshipResponse.Accept:
                    await CreateOrUpdateFriendAsync(inviter, invitee);
                    break;
                case FriendshipResponse.Mutual:
                {
                    await CreateOrUpdateFriendAsync(inviter, invitee);
                    // UI seems to still show mutual accept as an option even if already a friend
                    if (await invitee.GetFriendByIdentityAsync(inviter.Identity) == null)
                        _ = await _friendFactory.CreateFriendAsync(invitee, inviter, FriendshipType.Friend);

                    break;
                }
                case FriendshipResponse.Ignore:
                    _ = await _friendFactory.CreateFriendAsync(invitee, inviter, FriendshipType.Ignore);
                    break;
            }

            await invitee.RemoveFriendInviteAsync(invite);
            inviter.RemoveFriendInvitePending(invite);

            _friendManager.RemoveFriendInvite(invite);

            return null;
        }

        private async Task CreateOrUpdateFriendAsync(Character inviter, Character invitee)
        {
            Friend friend = await inviter.GetFriendByIdentityAsync(invitee.Identity);
            if (friend?.Type is FriendshipType.Rival)
                await friend.UpdateType(FriendshipType.FriendAndRival);
            else
                _ = await _friendFactory.CreateFriendAsync(inviter, invitee, FriendshipType.Friend);
        }
    }
}
