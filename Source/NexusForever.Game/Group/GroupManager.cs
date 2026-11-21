using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Group;
using NexusForever.Shared;
using InternalGroup = NexusForever.Network.Internal.Message.Group.Shared.Group;

namespace NexusForever.Game.Group
{
    public class GroupManager : IGroupManager
    {
        private readonly ReaderWriterLockSlim mutex = new();

        private readonly Dictionary<ulong, IGroup> groups = [];
        private readonly Dictionary<Identity, ulong> groupAssociations = [];

        #region Dependency Injection

        private readonly IFactory<IGroup> groupFactory;

        public GroupManager(
            IFactory<IGroup> groupFactory)
        {
            this.groupFactory = groupFactory;
        }

        #endregion

        public IGroup GetGroup(ulong id)
        {
            mutex.EnterReadLock();
            try
            {
                return groups.TryGetValue(id, out IGroup group) ? group : null;
            }
            finally
            {
                mutex.ExitReadLock();
            }
        }

        public IGroup GetGroup(Identity identity)
        {
            mutex.EnterReadLock();
            try
            {
                if (!groupAssociations.TryGetValue(identity, out ulong groupId))
                    return null;

                return groups.TryGetValue(groupId, out IGroup group) ? group : null;
            }
            finally
            {
                mutex.ExitReadLock();
            }
        }

        public void AddGroup(InternalGroup internalGroup)
        {
            mutex.EnterWriteLock();
            try
            {
                AddOrUpdateGroup(internalGroup);
            }
            finally
            {
                mutex.ExitWriteLock();
            }
        }

        private IGroup AddOrUpdateGroup(InternalGroup internalGroup)
        {
            IGroup group = groupFactory.Resolve();
            group.Initialise(internalGroup);
            groups[group.Id] = group;

            return group;
        }

        public void RemoveGroup(InternalGroup internalGroup)
        {
            mutex.EnterWriteLock();
            try
            {
                groups.Remove(internalGroup.Id);
            }
            finally
            {
                mutex.ExitWriteLock();
            }
        }

        public void AddGroupAssociation(InternalGroup internalGroup, Identity identity)
        {
            mutex.EnterWriteLock();
            try
            {
                IGroup group = AddOrUpdateGroup(internalGroup);
                groupAssociations[identity] = group.Id;
            }
            finally
            {
                mutex.ExitWriteLock();
            }
        }

        public void RemoveGroupAssociation(Identity identity)
        {
            mutex.EnterWriteLock();
            try
            {
                groupAssociations.Remove(identity);
            }
            finally
            {
                mutex.ExitWriteLock();
            }
        }
    }
}
