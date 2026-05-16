using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NexusForever.Database.Friendship;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Network.Internal.Message.Player;
using NexusForever.Server.Friendship.Game.Account;
using NexusForever.Server.Friendship.Game.Character;
using Rebus.Handlers;

namespace NexusForever.Server.Friendship.Network.Internal.Handler.Player
{
    public class PlayerLoggedInHandler : IHandleMessages<PlayerLoggedInMessage>
    {
        #region Dependency Injection

        private readonly FriendshipContext _context;
        private readonly IInternalMessagePublisher _messagePublisher;

        private readonly CharacterManager _characterManager;
        private readonly AccountManager _accountManager;

        public PlayerLoggedInHandler(
            FriendshipContext context,
            OutboxMessagePublisher outboxMessagePublisher,
            CharacterManager characterManager,
            AccountManager accountManager)
        {
            _context          = context;
            _messagePublisher = outboxMessagePublisher;
            _characterManager = characterManager;
            _accountManager   = accountManager;
        }

        #endregion

        public async Task Handle(PlayerLoggedInMessage message)
        {
            IExecutionStrategy strategy = _context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();

                Account account = await _accountManager.GetAccountRemoteAsync(message.AccountId);
                if (account == null)
                    return;

                await _context.SaveChangesAsync();

                Character character = await _characterManager.GetCharacterRemoteAsync(message.Identity.ToFriendshipIdentity());
                if (character == null)
                    return;

                await _context.SaveChangesAsync();

                await account.SetLastOnline(null);
                await account.SetActiveCharacterAsync(message.Identity.ToFriendshipIdentity());

                await character.SetLastOnline(null);

                await SendAccountMessages(account);
                await SendCharacterMessages(character);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            });
        }

        private async Task SendAccountMessages(Account account)
        {
            await account.SendFriendsAsync();
            await account.SendFriendInvitesAsync();

            await _messagePublisher.PublishAsync(new FriendshipAccountPersonalStatusUpdatedMessage
            {
                Account = await account.ToInternalAccountAsync(),
            });
        }

        private async Task SendCharacterMessages(Character character)
        {
            await character.SendFriendsAsync();
            await character.SendFriendInvitesAsync();
        }
    }
}
