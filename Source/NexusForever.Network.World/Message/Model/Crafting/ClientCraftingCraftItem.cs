using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Crafting
{
    [Message(GameMessageOpcode.ClientCraftingCraftItem)]
    public class ClientCraftingCraftItem : IReadable
    {
        public uint ClientSpellcastUniqueId { get; private set; }
        public uint CraftingStationUnitId { get; private set; }
        public uint TradeskillSchematic2Id { get; private set; }
        public uint SchematicCount { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ClientSpellcastUniqueId = reader.ReadUInt();
            CraftingStationUnitId = reader.ReadUInt();
            TradeskillSchematic2Id = reader.ReadUInt();
            SchematicCount = reader.ReadUInt(18);
        }
    }
}
