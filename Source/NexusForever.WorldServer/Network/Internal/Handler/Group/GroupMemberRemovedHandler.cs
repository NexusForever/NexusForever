using System.Threading.Tasks;
using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map.Instance;
using NexusForever.Game.Static.Map;
using NexusForever.Game.Static.Map.Lock;
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

            // a player who leaves the group inside its dungeon or raid gets the removal warning with the Leave button
            IPlayer removed = playerManager.GetPlayer(message.RemovedMember.Identity.ToGameIdentity());
            if (removed?.Map is IContentMapInstance instance && instance.MapLock?.Type is MapLockType.Group or MapLockType.Raid)
                instance.RequestPendingRemoval(removed, WorldRemovalReason.GroupMembership);

            return Task.CompletedTask;
        }
    }
}
