using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientCraftingRuneSlotReroll)]
    public class ClientCraftingRuneSlotReroll : IReadable
    {
        public ulong ItemGuid { get; private set; }
        public uint SlotIndex { get; private set; }
        public byte Field_C_5bit { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ItemGuid = reader.ReadULong();
            SlotIndex = reader.ReadUInt();
            Field_C_5bit = reader.ReadByte(5);
        }
    }
}
