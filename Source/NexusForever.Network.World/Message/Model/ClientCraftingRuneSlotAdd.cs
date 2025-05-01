using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientCraftingRuneSlotAdd)]
    public class ClientCraftingRuneSlotAdd : IReadable
    {
        public ulong ItemGuid { get; private set; }
        public bool IsNotFusion { get; private set; }
        public RuneType Type { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ItemGuid = reader.ReadULong();
            IsNotFusion = reader.ReadBit();
            Type = (RuneType)reader.ReadByte(5);
        }
    }
}
