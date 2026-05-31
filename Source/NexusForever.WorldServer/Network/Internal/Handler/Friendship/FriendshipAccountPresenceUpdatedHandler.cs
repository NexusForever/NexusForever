using System.Threading.Tasks;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Network.Internal.Message.Friendship.Shared;
using NexusForever.Network.World.Message.Model.Friendship;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Friendship
{
    public class FriendshipAccountPresenceUpdatedHandler : IHandleMessages<FriendshipAccountPresenceUpdatedMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public FriendshipAccountPresenceUpdatedHandler(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public Task Handle(FriendshipAccountPresenceUpdatedMessage message)
        {
            var friendshipAccountPresenceUpdate = new ServerFriendshipAccountPresenceUpdate
            {
                AccountId = message.Account.Id,
                Presence  = message.Account.Presence
            };

            IPlayer player = playerManager.GetPlayerByAccountId(message.Account.Id);
            player?.Session.EnqueueMessageEncrypted(friendshipAccountPresenceUpdate);

            foreach (FriendAccount friend in message.FriendsInverse)
            {
                IPlayer friendPlayer = playerManager.GetPlayerByAccountId(friend.InviterAccount.Id);
                friendPlayer?.Session.EnqueueMessageEncrypted(friendshipAccountPresenceUpdate);
            }

            return Task.CompletedTask;
        }
    }
}
