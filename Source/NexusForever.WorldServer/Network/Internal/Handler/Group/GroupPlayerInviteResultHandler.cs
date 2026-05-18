using System.Threading.Tasks;
using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Network.World.Message.Model;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Group
{
    public class GroupPlayerInviteResultHandler : IHandleMessages<GroupPlayerInviteResultMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public GroupPlayerInviteResultHandler(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public Task Handle(GroupPlayerInviteResultMessage message)
        {
            IPlayer player = playerManager.GetPlayer(message.Recipient.ToGameIdentity());
            player?.Session.EnqueueMessageEncrypted(new ServerGroupInviteResult
            {
                Name   = message.Target.Name,
                Result = message.Result
            });

            return Task.CompletedTask;
        }
    }
}
