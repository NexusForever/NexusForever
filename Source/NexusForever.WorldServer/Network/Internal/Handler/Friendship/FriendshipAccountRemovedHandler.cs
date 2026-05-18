using System.Threading.Tasks;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Network.World.Message.Model.Friendship;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Friendship
{
    public class FriendshipAccountRemovedHandler : IHandleMessages<FriendshipAccountRemovedMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public FriendshipAccountRemovedHandler(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public Task Handle(FriendshipAccountRemovedMessage message)
        {
            IPlayer player = playerManager.GetPlayerByAccountId(message.FriendAccount.InviterAccount.Id);
            player?.Session.EnqueueMessageEncrypted(new ServerFriendshipAccountRemoved
            {
                AccountFriendId = message.FriendAccount.Id
            });

            return Task.CompletedTask;
        }
    }
}
