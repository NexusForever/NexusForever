using System.Threading.Tasks;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Network.Internal.Message.Friendship.Shared;
using NexusForever.Network.World.Message.Model.Friendship;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Friendship
{
    public class FriendshipAccountUpdatedHandler : IHandleMessages<FriendshipAccountUpdatedMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public FriendshipAccountUpdatedHandler(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public Task Handle(FriendshipAccountUpdatedMessage message)
        {
            var friendshipAccountUpdate = new ServerFriendshipAccountUpdate
            {
                AccountId = message.Account.Id,
                State     = message.Account.Presence
            };

            if (message.Account.ActiveCharacter != null)
            {
                friendshipAccountUpdate.Characters.Add(new CharacterData
                {
                    PlayerIdentity = message.Account.ActiveCharacter.Identity.ToNetworkIdentity(),
                    Name           = message.Account.ActiveCharacter.IdentityName.Name,
                    Class          = message.Account.ActiveCharacter.Class,
                    Race           = message.Account.ActiveCharacter.Race,
                    Path           = message.Account.ActiveCharacter.Path,
                    Level          = message.Account.ActiveCharacter.Level,
                    WorldZoneId    = message.Account.ActiveCharacter.WorldZoneId,
                    Faction        = message.Account.ActiveCharacter.Faction
                });
            }

            foreach (FriendAccount friend in message.FriendsInverse)
            {
                IPlayer player = playerManager.GetPlayerByAccountId(friend.InviterAccount.Id);
                player?.Session.EnqueueMessageEncrypted(friendshipAccountUpdate);
            }

            return Task.CompletedTask;
        }
    }
}
