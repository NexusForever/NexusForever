using System.Threading.Tasks;
using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Network.World.Message.Model.Friendship;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Friendship
{
    public class FriendshipResultHandler : IHandleMessages<FriendshipResultMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public FriendshipResultHandler(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public Task Handle(FriendshipResultMessage message)
        {
            IPlayer player = playerManager.GetPlayer(message.Target.ToGameIdentity());
            player?.Session.EnqueueMessageEncrypted(new ServerFriendshipResult
            {
                Result = message.Result
            });

            return Task.CompletedTask;
        }
    }
}
