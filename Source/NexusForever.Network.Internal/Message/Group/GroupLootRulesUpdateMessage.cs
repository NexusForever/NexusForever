using NexusForever.Game.Static.Group;
using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Group
{
    public class GroupLootRulesUpdateMessage
    {
        public ulong GroupId { get; set; }
        public Identity Identity { get; set; }
        public LootRule NormalRule { get; set; }
        public LootRule ThresholdRule { get; set; }
        public LootThreshold ThresholdQuality { get; set; }
        public HarvestLootRule HarvestRule { get; set; }
    }
}
