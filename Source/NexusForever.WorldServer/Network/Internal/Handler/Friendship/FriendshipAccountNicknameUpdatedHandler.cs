using System.Threading.Tasks;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Network.Internal.Message.Friendship.Shared;
using NexusForever.Network.World.Message.Model.Friendship;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Friendship
{
    public class FriendshipAccountNicknameUpdatedHandler : IHandleMessages<FriendshipAccountNicknameUpdatedMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public FriendshipAccountNicknameUpdatedHandler(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public Task Handle(FriendshipAccountNicknameUpdatedMessage message)
        {
            var friendshipAccountDisplayNameUpdate = new ServerFriendshipAccountDisplayNameUpdate
            {
                AccountId   = message.Account.Id,
                DisplayName = message.Account.Nickname ?? message.Account.Email,
            };

            IPlayer player = playerManager.GetPlayerByAccountId(message.Account.Id);
            player?.Session.EnqueueMessageEncrypted(friendshipAccountDisplayNameUpdate);

            foreach (FriendAccount friend in message.FriendsInverse)
            {
                IPlayer friendPlayer = playerManager.GetPlayerByAccountId(friend.InviterAccount.Id);
                friendPlayer?.Session.EnqueueMessageEncrypted(friendshipAccountDisplayNameUpdate);
            }

            return Task.CompletedTask;
        }
    }
}
