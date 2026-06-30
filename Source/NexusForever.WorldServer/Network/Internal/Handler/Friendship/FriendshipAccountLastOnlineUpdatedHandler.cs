using System;
using System.Threading.Tasks;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Network.Internal.Message.Friendship.Shared;
using NexusForever.Network.World.Message.Model.Friendship;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Friendship
{
    public class FriendshipAccountLastOnlineUpdatedHandler : IHandleMessages<FriendshipAccountLastOnlineUpdatedMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public FriendshipAccountLastOnlineUpdatedHandler(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public Task Handle(FriendshipAccountLastOnlineUpdatedMessage message)
        {
            var friendshipAccountLastOnlineUpdate = new ServerFriendshipAccountLastOnlineUpdate
            {
                AccountId = message.Account.Id
            };

            if (message.Account.LastOnline != null)
                friendshipAccountLastOnlineUpdate.DaysSinceLastOnline = (float)(DateTime.UtcNow - message.Account.LastOnline.Value).TotalDays;

            foreach (FriendAccount friend in message.FriendsInverse)
            {
                IPlayer player = playerManager.GetPlayerByAccountId(friend.InviterAccount.Id);
                player?.Session.EnqueueMessageEncrypted(friendshipAccountLastOnlineUpdate);
            }

            return Task.CompletedTask;
        }
    }
}
