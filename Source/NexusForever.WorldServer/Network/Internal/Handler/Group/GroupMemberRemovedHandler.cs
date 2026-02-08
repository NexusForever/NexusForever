using System.Threading.Tasks;
using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Network.Internal.Message.Group.Shared;
using NexusForever.Network.World.Message.Model;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Group
{
    public class GroupMemberRemovedHandler : IHandleMessages<GroupMemberRemovedMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public GroupMemberRemovedHandler(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public Task Handle(GroupMemberRemovedMessage message)
        {
            var groupRemove = new ServerGroupRemove
            {
                GroupId      = message.Group.Id,
                Reason       = message.Reason,
                TargetPlayer = message.RemovedMember.Identity.ToNetworkIdentity(),
            };

            foreach (GroupMember member in message.Group.Members)
            {
                IPlayer player = playerManager.GetPlayer(member.Identity.ToGameIdentity());
                player?.Session.EnqueueMessageEncrypted(groupRemove);
            }

            return Task.CompletedTask;
        }
    }
}
