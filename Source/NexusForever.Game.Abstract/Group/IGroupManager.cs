using NexusForever.Game.Abstract.Entity;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Group
{
    public interface IGroupManager : IUpdate
    {
        IGroup CreateGroup(IPlayer player);
        bool FindGroupMembershipsForPlayer(IPlayer player, out IGroupMember membership1, out IGroupMember membership2);
        IGroup GetGroupById(ulong groupId);
        IGroup GetGroupByLeader(IPlayer player);
        void RemoveGroup(IGroup group);
        void RestoreGroupsForPlayer(IPlayer player);
    }
}