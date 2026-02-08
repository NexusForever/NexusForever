using System.Threading.Tasks;
using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Group;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Network.World.Message.Model;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Group
{
    public class GroupMemberRequestResultHandler : IHandleMessages<GroupMemberRequestResultMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public GroupMemberRequestResultHandler(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public Task Handle(GroupMemberRequestResultMessage message)
        {
            IPlayer player = playerManager.GetPlayer(message.Recipient.ToGameIdentity());
            player?.Session.EnqueueMessageEncrypted(new ServerGroupRequestJoinResult
            {
                IsJoin = message.Type == GroupRequestType.Request,
                Name   = message.Target.Name,
                Result = message.Result,
            });

            return Task.CompletedTask;
        }
    }
}
