using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientCraftingCompleteComplexCraft)]
    public class ClientCraftingCompleteComplexCraft : IReadable
    {
        public uint ClientSpellcastUniqueId { get; private set; }
        public uint CraftingStationUnitId { get; private set; }
        public uint TradeskillSchematic2Id { get; private set; }
        public CraftStats Stats { get; private set; } = new CraftStats();
        public uint PowerCoreItem2Id { get; private set; }
        public uint ApSpSplitDelta { get; private set; }
        public int[] UnknownArray { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ClientSpellcastUniqueId = reader.ReadUInt();
            CraftingStationUnitId = reader.ReadUInt();
            TradeskillSchematic2Id = reader.ReadUInt();
            Stats.Read(reader);
            PowerCoreItem2Id = reader.ReadUInt(18);
            ApSpSplitDelta = reader.ReadUInt();

            uint count = reader.ReadUInt(3);
            UnknownArray = new int[count];
            for (int i = 0; i < count; i++)
            {
                UnknownArray[i] = reader.ReadInt();
            }
        }
    }
}
