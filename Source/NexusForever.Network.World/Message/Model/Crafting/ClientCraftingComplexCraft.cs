using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Crafting
{
    [Message(GameMessageOpcode.ClientCraftingComplexCraft)]
    public class ClientCraftingComplexCraft : IReadable
    {
        public uint ClientSpellcastUniqueId { get; private set; }
        public uint CraftingStationUnitId { get; private set; }
        public uint TradeskillSchematic2Id { get; private set; }
        public CraftStats CraftStats { get; private set; } = new CraftStats();
        public uint PowerCoreItem2Id { get; private set; }
        public uint ApSpSplitDelta { get; private set; }
        public int[] ChargeCounts { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ClientSpellcastUniqueId = reader.ReadUInt();
            CraftingStationUnitId = reader.ReadUInt();
            TradeskillSchematic2Id = reader.ReadUInt();
            CraftStats.Read(reader);
            PowerCoreItem2Id = reader.ReadUInt(18);
            ApSpSplitDelta = reader.ReadUInt();

            uint count = reader.ReadUInt(3);
            ChargeCounts = new int[count];
            for (int i = 0; i < count; i++)
            {
                ChargeCounts[i] = reader.ReadInt();
            }
        }
    }
}
