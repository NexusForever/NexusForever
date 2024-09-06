using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Group;
using NexusForever.Game.Entity;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Model.Shared;
using NexusForever.Shared;

namespace NexusForever.Game.Group
{
    public sealed class GroupManager : Singleton<GroupManager>, IGroupManager
    {
        private Dictionary<ulong, IGroup> groups = new Dictionary<ulong, IGroup>();
        private Dictionary<ulong, IGroup> groupOwner = new Dictionary<ulong, IGroup>();

        /// <summary>
        /// Create a <see cref="Group"/> for supplied <see cref="Player"/>
        /// </summary>
        public IGroup CreateGroup(IPlayer player)
        {
            // Player is already leader in a group
            if (groupOwner.ContainsKey(player.CharacterId))
                return null;

            IGroup group = Group.CreateOpenWorld(NextGroupId(), player);
            groups.Add(group.Id, group);
            groupOwner.Add(player.CharacterId, group);

            return group;
        }

        /// <summary>
        /// Removes the Group from the Session.
        /// </summary>
        /// <param name="group">The group to remove.</param>
        public void RemoveGroup(IGroup group)
        {
            if (!groups.ContainsKey(group.Id))
                return;

            groups.Remove(group.Id);
            groupOwner.Remove(group.Leader.CharacterId);
        }

        /// <summary>
        /// Get the current <see cref="Group"/> from the supplied <see cref="Player"/>
        /// </summary>
        public IGroup GetGroupByLeader(IPlayer player)
        {
            if (!groupOwner.TryGetValue(player.CharacterId, out var group))
                return null;

            return group;
        }

        /// <summary>
        /// Get a <see cref="Group"/> with the supplied <see cref="ulong"/> Group Id
        /// </summary>
        public IGroup GetGroupById(ulong groupId)
        {
            if (!groups.TryGetValue(groupId, out IGroup group))
                return null;

            return group;
        }

        public bool FindGroupMembershipsForPlayer(IPlayer player, out IGroupMember membership1, out IGroupMember membership2)
        {
            membership1 = null;
            membership2 = null;

            foreach (IGroup group in groups.Values)
            {
                IGroupMember membership = group.FindMember(new TargetPlayerIdentity() { CharacterId = player.CharacterId, RealmId = RealmContext.Instance.RealmId });

                if (membership == null)
                    continue;

                if (!membership.Group.IsOpenWorld) // Is Instance Group.
                {
                    if (membership1 != null)
                        throw new System.InvalidOperationException("Error in restoring groups for player. Player cannot be part of two Instance groups.");

                    // If this is an instance group it MUST be group1.
                    membership1 = membership;
                }

                if (membership1 == null)
                {
                    membership1 = membership;
                    continue;
                }

                if (membership2 != null)
                    throw new System.InvalidOperationException("Error in restoring groups for player. Player cannot be part of more than two groups.");

                membership2 = membership;
            }

            return membership1 != null;
        }

        public void RestoreGroupsForPlayer(IPlayer player)
        {
            if (!FindGroupMembershipsForPlayer(player, out IGroupMember membership, out IGroupMember membership2))
                return;

            // Add the player back to the other group IF they are a member of one first.
            if (membership2 != null)
            {
                player.AddToGroup(membership2);
                player.Session.EnqueueMessageEncrypted(new ServerGroupJoin
                {
                    TargetPlayer = new TargetPlayerIdentity
                    {
                        CharacterId = player.CharacterId,
                        RealmId = RealmContext.Instance.RealmId
                    },
                    GroupInfo = membership2.Group.Build()
                });
            }

            // Then add them back to the main group.
            player.AddToGroup(membership);
            player.Session.EnqueueMessageEncrypted(new ServerGroupJoin
            {
                TargetPlayer = new TargetPlayerIdentity
                {
                    CharacterId = player.CharacterId,
                    RealmId = RealmContext.Instance.RealmId
                },
                GroupInfo = membership.Group.Build()
            });
        }

        public void Update(double lastTick)
        {
            foreach (var group in groups.Values)
                group.Update(lastTick);
        }

        /// <summary>
        /// Generate a new GroupId.
        /// </summary>
        private ulong NextGroupId()
        {
            if (groups.Count > 0)
                return groups.Last().Key + 1;
            else
                return 1;
        }
    }
}
