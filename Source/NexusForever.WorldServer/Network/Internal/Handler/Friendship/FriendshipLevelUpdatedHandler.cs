using System.Threading.Tasks;
using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Network.Internal.Message.Friendship.Shared;
using NexusForever.Network.World.Message.Model.Friendship;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Friendship
{
    public class FriendshipLevelUpdatedHandler : IHandleMessages<FriendshipLevelUpdatedMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public FriendshipLevelUpdatedHandler(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public Task Handle(FriendshipLevelUpdatedMessage message)
        {
            var friendshipLevelUpdate = new ServerFriendshipLevelUpdate
            {
                Character = message.Character.Identity.ToNetworkIdentity(),
                Level     = message.Character.Level
            };

            foreach (Friend friend in message.FriendsInverse)
            {
                IPlayer player = playerManager.GetPlayer(friend.InviterCharacter.Identity.ToGameIdentity());
                player?.Session.EnqueueMessageEncrypted(friendshipLevelUpdate);
            }

            return Task.CompletedTask;
        }
    }
}
