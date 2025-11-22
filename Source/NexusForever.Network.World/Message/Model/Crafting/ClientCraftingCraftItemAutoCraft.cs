using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Crafting
{
    [Message(GameMessageOpcode.ClientCraftingCraftItemAutoCraft)]
    public class ClientCraftingCraftItemAutoCraft : IReadable
    {
        public uint ClientSpellcastUniqueId { get; private set; } // creates a spell cast when the craft is Runecrafting
        public uint CraftingStationUnitId { get; private set; }
        public uint TradeskillSchematic2Id { get; private set; }
        public uint SchematicCount { get; private set; }
        public uint CatalystItem2Id { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ClientSpellcastUniqueId = reader.ReadUInt();
            CraftingStationUnitId = reader.ReadUInt();
            TradeskillSchematic2Id = reader.ReadUInt();
            SchematicCount = reader.ReadUInt();
            CatalystItem2Id = reader.ReadUInt(18);
        }
    }
}
