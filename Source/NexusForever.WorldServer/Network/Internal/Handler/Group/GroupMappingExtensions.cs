using System.Linq;
using NexusForever.Network.World.Message.Model.Shared;
using InternalGroup = NexusForever.Network.Internal.Message.Group.Shared.Group;
using NetworkGroup = NexusForever.Network.World.Message.Model.Shared.Group;

namespace NexusForever.WorldServer.Network.Internal.Handler.Group
{
    public static class GroupMappingExtensions
    {
        public static NetworkGroup ToNetworkGroup(this InternalGroup group)
        {
            return new NetworkGroup
            {
                GroupId           = group.Id,
                Flags             = group.Flags,
                MaxGroupSize      = group.MaxGroupSize,
                LootRule          = group.NormalRule,
                LootRuleThreshold = group.ThresholdRule,
                LootThreshold     = group.ThresholdQuality,
                LootRuleHarvest   = group.HarvestRule,
                LeaderIdentity    = group.Leader.ToNetworkIdentity(),
                MemberInfos       = group.Members.Select(member => member.ToNetworkGroupMember()).ToList(),
                MarkerInfo        = new GroupMarkerInfo
                {
                    Markers = group.Markers.Select(marker => new MarkerInfo
                    {
                        UnitId = marker.UnitId,
                        Marker = marker.Marker
                    }).ToArray()
                }
            };
        }
    }
}
