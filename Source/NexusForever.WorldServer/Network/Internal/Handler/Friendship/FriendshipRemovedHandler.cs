using System.Threading.Tasks;
using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Network.World.Message.Model.Friendship;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Friendship
{
    public class FriendshipRemovedHandler : IHandleMessages<FriendshipRemovedMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public FriendshipRemovedHandler(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public Task Handle(FriendshipRemovedMessage message)
        {
            IPlayer player = playerManager.GetPlayer(message.Friend.InviterCharacter.Identity.ToGameIdentity());
            player?.Session.EnqueueMessageEncrypted(new ServerFriendshipRemove
            {
                FriendshipId = message.Friend.Id
            });

            return Task.CompletedTask;
        }
    }
}
