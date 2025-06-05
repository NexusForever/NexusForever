using NexusForever.Network.Message;
using NexusForever.Game.Static.Group;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGroupLootRulesChanged)]
    public class ServerGroupLootRulesChanged : IWritable
    {
        public ulong GroupId { get; set; }
        public uint Unused { get; set; } // Unpacked but unused by Client
        public LootRule LootRulesUnderThreshold { get; set; }
        public LootRule LootRulesThresholdAndOver { get; set; }
        public LootThreshold LootThreshold { get; set; }
        public HarvestLootRule HarvestLootRule { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GroupId);
            writer.Write(Unused);
            writer.Write(LootRulesUnderThreshold, 3u);
            writer.Write(LootRulesThresholdAndOver, 3u);
            writer.Write(LootThreshold, 4u);
            writer.Write(HarvestLootRule, 2u);
        }
    }
}
