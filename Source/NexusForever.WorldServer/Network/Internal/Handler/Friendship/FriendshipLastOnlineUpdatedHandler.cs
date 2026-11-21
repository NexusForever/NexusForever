using System;
using System.Threading.Tasks;
using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Network.Internal.Message.Friendship.Shared;
using NexusForever.Network.World.Message.Model.Friendship;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Friendship
{
    public class FriendshipLastOnlineUpdatedHandler : IHandleMessages<FriendshipLastOnlineUpdatedMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public FriendshipLastOnlineUpdatedHandler(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public Task Handle(FriendshipLastOnlineUpdatedMessage message)
        {
            var friendshipLastOnlineUpdate = new ServerFriendshipLastOnlineUpdate
            {
                PlayerIdentity = message.Character.Identity.ToNetworkIdentity()
            };

            if (message.Character.LastOnline != null)
                friendshipLastOnlineUpdate.LastOnlineInDays = (float)(message.Character.LastOnline.Value - DateTime.UtcNow).TotalDays;

            foreach (Friend friend in message.FriendsInverse)
            {
                IPlayer player = playerManager.GetPlayer(friend.InviterCharacter.Identity.ToGameIdentity());
                player?.Session.EnqueueMessageEncrypted(friendshipLastOnlineUpdate);
            }

            return Task.CompletedTask;
        }
    }
}
