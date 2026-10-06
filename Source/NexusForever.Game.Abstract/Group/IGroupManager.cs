using InternalGroup = NexusForever.Network.Internal.Message.Group.Shared.Group;

namespace NexusForever.Game.Abstract.Group
{
    public interface IGroupManager
    {
        IGroup GetGroup(ulong id);

        IGroup GetGroup(Identity identity);

        void AddGroup(InternalGroup internalGroup);

        void RemoveGroup(InternalGroup internalGroup);

        void AddGroupAssociation(InternalGroup internalGroup, Identity identity);

        void RemoveGroupAssociation(Identity identity);
    }
}
