using NexusForever.Database.Friendship;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Network.Internal.Message.Player;
using NexusForever.Server.Friendship.Game.Account;
using NexusForever.Server.Friendship.Game.Character;
using NexusForever.Server.Friendship.Game.Friend;
using Rebus.Handlers;

namespace NexusForever.Server.Friendship.Network.Internal.Handler.Player
{
    public class PlayerStatUpdatedHandler : IHandleMessages<PlayerStatUpdatedMessage>
    {
        #region Dependency Injection

        private readonly FriendshipContext _context;
        private readonly IInternalMessagePublisher _messagePublisher;
        private readonly CharacterManager _characterManager;

        public PlayerStatUpdatedHandler(
            FriendshipContext friendContext,
            OutboxMessagePublisher messagePublisher,
            CharacterManager characterManager)
        {
            _context          = friendContext;
            _messagePublisher = messagePublisher;
            _characterManager = characterManager;
        }

        #endregion

        public async Task Handle(PlayerStatUpdatedMessage message)
        {
            // we only care about level updates, other stats are not relevant for the friendship server
            if (message.Stat != Stat.Level)
                return;

            Character character = await _characterManager.GetCharacterAsync(message.Identity.ToFriendshipIdentity());
            if (character == null)
                return;

            Account account = await character.GetAccountAsync();
            if (account == null)
                return;

            await character.SetLevelAsync((byte)message.Value);

            var friendshipAccountLevelUpdated = new FriendshipAccountLevelUpdatedMessage
            {
                Account = await account.ToInternalAccountAsync()
            };

            foreach (var friend in await account.GetInternalFriendsInverseAsync())
                friendshipAccountLevelUpdated.FriendsInverse.Add(friend);

            await _messagePublisher.PublishAsync(friendshipAccountLevelUpdated);

            await _context.SaveChangesAsync();
        }
    }
}
