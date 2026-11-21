﻿using System.Collections;
using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Group;
using NexusForever.Game.Static.Group;
using NexusForever.Game.Static.Item;
using NexusForever.Shared;
using InternalGroup = NexusForever.Network.Internal.Message.Group.Shared.Group;
using InternalGroupMember = NexusForever.Network.Internal.Message.Group.Shared.GroupMember;

namespace NexusForever.Game.Group
{
    public class Group : IGroup
    {
        public ulong Id { get; private set; }
        public LootRule NormalRule { get; private set; }
        public LootRule ThresholdRule { get; private set; }
        public Quality ThresholdQuality { get; private set; }
        public HarvestLootRule HarvestRule { get; private set; }
        public Identity Leader { get; private set; }

        private readonly Dictionary<Identity, IGroupMember> members = [];

        #region Dependency Injection

        private readonly IFactory<IGroupMember> groupMemberFactory;

        public Group(
            IFactory<IGroupMember> groupMemberFactory)
        {
            this.groupMemberFactory = groupMemberFactory;
        }

        #endregion

        public void Initialise(InternalGroup internalGroup)
        {
            Id               = internalGroup.Id;
            NormalRule       = internalGroup.NormalRule;
            ThresholdRule    = internalGroup.ThresholdRule;
            ThresholdQuality = (Quality)internalGroup.ThresholdQuality;
            Leader           = internalGroup.Leader.ToGameIdentity();

            foreach (InternalGroupMember internalGroupMember in internalGroup.Members)
            {
                IGroupMember groupMember = groupMemberFactory.Resolve();
                groupMember.Initialise(internalGroupMember);
                members.Add(groupMember.Identity, groupMember);
            }
        }

        public IGroupMember GetMember(Identity identity)
        {
            return members.TryGetValue(identity, out IGroupMember groupMember) ? groupMember : null;
        }

        public IEnumerator<IGroupMember> GetEnumerator()
        {
            return members.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
