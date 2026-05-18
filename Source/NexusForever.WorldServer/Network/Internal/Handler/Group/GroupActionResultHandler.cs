using System.Threading.Tasks;
using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Network.World.Message.Model;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Group
{
    public class GroupActionResultHandler : IHandleMessages<GroupActionResultMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public GroupActionResultHandler(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public Task Handle(GroupActionResultMessage message)
        {
            IPlayer player = playerManager.GetPlayer(message.Recipient.ToGameIdentity());
            player?.Session.EnqueueMessageEncrypted(new ServerGroupActionResult
            {
                GroupId  = message.GroupId,
                Identity = message.Target.ToNetworkIdentity(),
                Result   = message.Result
            });

            return Task.CompletedTask;
        }
    }
}
