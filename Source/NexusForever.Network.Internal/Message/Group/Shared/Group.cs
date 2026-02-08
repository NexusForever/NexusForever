using NexusForever.Game.Static.Group;
using NexusForever.Game.Static.Matching;
using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Group.Shared
{
    public class Group
    {
        public ulong Id { get; set; }
        public GroupFlags Flags { get; set; }
        public LootRule NormalRule { get; set; }
        public LootRule ThresholdRule { get; set; }
        public LootThreshold ThresholdQuality { get; set; }
        public HarvestLootRule HarvestRule { get; set; }
        public Identity Leader { get; set; }
        public uint MaxGroupSize { get; set; }
        public Guid? Match { get; set; }
        public MatchTeam? MatchTeam { get; set; }
        public GroupRequest Request { get; set; }
        public List<GroupMarker> Markers { get; set; }
        public List<GroupMember> Members { get; set; }
    }
}
