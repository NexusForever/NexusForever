using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientCraftingCompleteComplexCraft)]
    public class ClientCraftingCompleteComplexCraft : IReadable
    {
        public uint ClientSpellcastUniqueId { get; private set; }
        public uint CraftingStationUnitId { get; private set; }
        public uint TradeskillSchematic2Id { get; private set; }
        public ulong Field_10_64bit { get; private set; }
        public uint Field_18_18bit { get; private set; }
        public uint Field_1C_32bit { get; private set; }
        public uint[] UnknownArray { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ClientSpellcastUniqueId = reader.ReadUInt();
            CraftingStationUnitId = reader.ReadUInt();
            TradeskillSchematic2Id = reader.ReadUInt();
            Field_10_64bit = reader.ReadULong();
            Field_18_18bit = reader.ReadUInt(18);
            Field_1C_32bit = reader.ReadUInt();

            uint count = reader.ReadUInt(3);
            UnknownArray = new uint[count];
            for (int i = 0; i < count; i++)
            {
                UnknownArray[i] = reader.ReadUInt();
            }
        }
    }
}
