using NexusForever.Game.Static.Group;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientGroupSetLootRules)]
    public class ClientGroupSetLootRules : IReadable
    {
        public ulong GroupId { get; private set; }
        public LootRule LootRulesUnderThreshold { get; private set; }
        public LootRule LootRulesThresholdAndOver { get; private set; }
        public LootThreshold Threshold { get; private set; }
        public HarvestLootRule HarvestingRule { get; private set; }

        public void Read(GamePacketReader reader)
        {
            GroupId = reader.ReadULong();
            LootRulesUnderThreshold = reader.ReadEnum<LootRule>(3u);
            LootRulesThresholdAndOver = reader.ReadEnum<LootRule>(3u);
            Threshold = reader.ReadEnum<LootThreshold>(4u);
            HarvestingRule = reader.ReadEnum<HarvestLootRule>(2u);
        }
    }
}
