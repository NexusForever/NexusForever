using System.Threading.Tasks;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Network.Internal.Message.Friendship.Shared;
using NexusForever.Network.World.Message.Model.Friendship;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Friendship
{
    public class FriendshipAccountLevelUpdatedHandler : IHandleMessages<FriendshipAccountLevelUpdatedMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public FriendshipAccountLevelUpdatedHandler(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public Task Handle(FriendshipAccountLevelUpdatedMessage message)
        {
            var friendLevelUpdate = new ServerFriendshipAccountCharacterLevelUpdate
            {
                AccountId = message.Account.Id,
                Character = message.Account.ActiveCharacter.Identity.ToNetworkIdentity(),
                Level     = message.Account.ActiveCharacter.Level
            };

            foreach (FriendAccount friend in message.FriendsInverse)
            {
                IPlayer player = playerManager.GetPlayerByAccountId(friend.InviterAccount.Id);
                player?.Session.EnqueueMessageEncrypted(friendLevelUpdate);
            }

            return Task.CompletedTask;
        }
    }
}
