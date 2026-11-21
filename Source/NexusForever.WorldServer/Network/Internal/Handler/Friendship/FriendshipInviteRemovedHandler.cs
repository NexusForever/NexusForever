using System.Threading.Tasks;
using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Network.World.Message.Model.Friendship;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Friendship
{
    public class FriendshipInviteRemovedHandler : IHandleMessages<FriendshipInviteRemovedMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public FriendshipInviteRemovedHandler(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public Task Handle(FriendshipInviteRemovedMessage message)
        {
            IPlayer player = playerManager.GetPlayer(message.FriendInvite.Character.Identity.ToGameIdentity());
            player.Session.EnqueueMessageEncrypted(new ServerFriendshipInviteRemove
            {
                InviteId = message.FriendInvite.Id
            });

            return Task.CompletedTask;
        }
    }
}
