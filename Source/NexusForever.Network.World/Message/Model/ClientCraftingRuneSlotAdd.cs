using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientCraftingRuneSlotAdd)]
    public class ClientCraftingRuneSlotAdd : IReadable
    {
        public ulong ItemGuid { get; private set; }
        public bool Field_8_1bit { get; private set; }
        public byte SlotIndex_5bit { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ItemGuid = reader.ReadULong();
            Field_8_1bit = reader.ReadBit();
            SlotIndex_5bit = reader.ReadByte(5);
        }
    }
}
