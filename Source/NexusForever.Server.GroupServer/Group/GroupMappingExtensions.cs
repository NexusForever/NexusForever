using InternalGroup = NexusForever.Network.Internal.Message.Group.Shared.Group;
using InternalGroupMarker = NexusForever.Network.Internal.Message.Group.Shared.GroupMarker;
using InternalGroupMember = NexusForever.Network.Internal.Message.Group.Shared.GroupMember;
using InternalGroupRequest = NexusForever.Network.Internal.Message.Group.Shared.GroupRequest;

namespace NexusForever.Server.GroupServer.Group
{
    public static class GroupMappingExtensions
    {
        public static async Task<InternalGroup> ToInternalGroup(this Group group)
        {
            var members = new List<InternalGroupMember>();
            foreach (GroupMember member in group.GetMembers())
                members.Add(await member.ToInternalGroupMember());

            var internalGroup = new InternalGroup
            {
                Id               = group.Id,
                Flags            = group.Flags,
                NormalRule       = group.LootRule,
                ThresholdRule    = group.LootRuleThreshold,
                ThresholdQuality = group.LootThreshold,
                HarvestRule      = group.LootRuleHarvest,
                Leader           = group.Leader?.ToInternalIdentity(),
                MaxGroupSize     = group.GetMaxGroupSize(),
                Match            = group.Match,
                MatchTeam        = group.MatchTeam,
                Members          = members,
                Markers          = group.GetMarkers().Select(m => new InternalGroupMarker
                {
                    Marker = m.Marker,
                    UnitId = m.UnitId
                }).ToList()
            };

            if (group.Request != null)
            {
                internalGroup.Request = new InternalGroupRequest
                {
                    Requester  = group.Request.RequesterIdentity.ToInternalIdentity(),
                    Requestee  = group.Request.RequesteeIdentity.ToInternalIdentity(),
                    Type       = group.Request.Type,
                    Expiration = group.Request.Expiration,
                };
            }

            return internalGroup;
        }
    }
}
