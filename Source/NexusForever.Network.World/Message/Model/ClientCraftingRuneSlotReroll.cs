using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientCraftingRuneSlotReroll)]
    public class ClientCraftingRuneSlotReroll : IReadable
    {
        public ulong ItemGuid { get; private set; }
        public uint SlotIndex { get; private set; }
        public RuneType Type { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ItemGuid = reader.ReadULong();
            SlotIndex = reader.ReadUInt();
            Type = (RuneType)reader.ReadByte(5);
        }
    }
}
