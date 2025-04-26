using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientCraftingRuneSlotClear)]
    public class ClientCraftingRuneSlotClear : IReadable
    {
        public ulong ItemGuid { get; private set; }
        public uint RuneSlotIndex { get; private set; }
        public bool Field_C_1bit { get; private set; }
        public bool Field_10_1bit { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ItemGuid = reader.ReadULong();
            RuneSlotIndex = reader.ReadUInt();
            Field_C_1bit = reader.ReadBit();
            Field_10_1bit = reader.ReadBit();
        }
    }
}
