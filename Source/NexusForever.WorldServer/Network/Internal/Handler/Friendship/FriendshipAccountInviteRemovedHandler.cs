using System.Threading.Tasks;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Network.World.Message.Model.Friendship;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Friendship
{
    public class FriendshipAccountInviteRemovedHandler : IHandleMessages<FriendshipAccountInviteRemovedMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public FriendshipAccountInviteRemovedHandler(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public Task Handle(FriendshipAccountInviteRemovedMessage message)
        {
            IPlayer player = playerManager.GetPlayerByAccountId(message.FriendInvite.InviteeAccount.Id);
            player?.Session.EnqueueMessageEncrypted(new ServerFriendshipAccountInviteRemove
            {
                AccountId             = player.Account.Id,
                AccountFriendInviteId = message.FriendInvite.Id
            });

            return Task.CompletedTask;
        }
    }
}
