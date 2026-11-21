using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Server.Friendship.Game.Account;
using NexusForever.Server.Friendship.Game.Character;
using NexusForever.Server.Friendship.Game.Friend;
using Rebus.Handlers;

namespace NexusForever.Server.Friendship.Network.Internal.Handler.Friendship
{
    public class FriendshipLocationRequestHandler : IHandleMessages<FriendshipLocationRequestMessage>
    {
        #region Dependency Injection

        private readonly CharacterManager _characterManager;
        private readonly IInternalMessagePublisher _messagePublisher;

        public FriendshipLocationRequestHandler(
            CharacterManager characterManager,
            IInternalMessagePublisher messagePublisher)
        {
            _characterManager = characterManager;
            _messagePublisher = messagePublisher;
        }

        #endregion

        public async Task Handle(FriendshipLocationRequestMessage message)
        {
            Character character = await _characterManager.GetCharacterAsync(message.Identity.ToFriendshipIdentity());
            if (character == null)
                return;

            if (message.Friends.Count != 0)
            {
                var friendshipLocationsUpdatedMessage = new FriendshipLocationsUpdatedMessage
                {
                    Character = character.ToInternalCharacter()
                };

                foreach (var identity in message.Friends)
                {
                    Friend friend = await character.GetFriendByIdentityAsync(identity.ToFriendshipIdentity());
                    if (friend != null)
                        friendshipLocationsUpdatedMessage.Friends.Add(await friend.ToInternalFriendAsync());
                }

                await _messagePublisher.PublishAsync(friendshipLocationsUpdatedMessage);
            }

            if (message.AccountFriends.Count != 0)
            {
                Account account = await character.GetAccountAsync();
                if (account == null)
                    return;

                var friendshipAccountLocationsUpdated = new FriendshipAccountLocationsUpdatedMessage
                {
                    Account = await account.ToInternalAccountAsync()
                };

                foreach (ulong friendAccountId in message.AccountFriends)
                {
                    FriendAccount friend = await account.GetFriendAsync(friendAccountId);
                    if (friend != null)
                        friendshipAccountLocationsUpdated.Friends.Add(await friend.ToInternalFriendAsync());
                }

                await _messagePublisher.PublishAsync(friendshipAccountLocationsUpdated);
            }
        }
    }
}
