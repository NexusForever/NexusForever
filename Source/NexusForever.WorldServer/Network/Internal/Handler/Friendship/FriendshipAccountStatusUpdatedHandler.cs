using System.Threading.Tasks;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Network.Internal.Message.Friendship.Shared;
using NexusForever.Network.World.Message.Model.Friendship;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Friendship
{
    public class FriendshipAccountStatusUpdatedHandler : IHandleMessages<FriendshipAccountStatusUpdatedMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public FriendshipAccountStatusUpdatedHandler(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public Task Handle(FriendshipAccountStatusUpdatedMessage message)
        {
            var friendshipAccountPublicNote = new ServerFriendshipAccountPublicNote
            {
                AccountId  = message.Account.Id,
                PublicNote = message.Account?.Status ?? string.Empty
            };

            foreach (FriendAccount friend in message.FriendsInverse)
            {
                IPlayer player = playerManager.GetPlayerByAccountId(friend.InviterAccount.Id);
                player?.Session.EnqueueMessageEncrypted(friendshipAccountPublicNote);
            }

            return Task.CompletedTask;
        }
    }
}
