using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Network.Internal.Message.Group.Shared;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Message.Model;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Group
{
    public class GroupMemberPositionUpdatedHandler : IHandleMessages<GroupMemberPositionUpdatedMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public GroupMemberPositionUpdatedHandler(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public Task Handle(GroupMemberPositionUpdatedMessage message)
        {
            foreach (IGrouping<uint, GroupMember> item in message.UpdatedMembers.GroupBy(m => m.Character.WorldId))
            {
                var groupPositionUpdate = new ServerGroupPositionUpdate
                {
                    GroupId = message.Group.Id,
                    WorldId = item.Key,
                };

                foreach (GroupMember groupMember in item)
                {
                    groupPositionUpdate.Updates.Add(new ServerGroupPositionUpdate.UnknownStruct0
                    {
                        Identity    = groupMember.Identity.ToNetworkIdentity(),
                        Position    = new Position(new Vector3(groupMember.Character.Position.X, groupMember.Character.Position.Y, groupMember.Character.Position.Z)),
                        WorldZoneId = groupMember.Character.WorldZoneId,
                        Flags       = 0
                    });
                }

                foreach (GroupMember groupMember in message.Group.Members)
                {
                    IPlayer player = playerManager.GetPlayer(groupMember.Identity.ToGameIdentity());
                    player?.Session.EnqueueMessageEncrypted(groupPositionUpdate);
                }
            }

            return Task.CompletedTask;
        }
    }
}
