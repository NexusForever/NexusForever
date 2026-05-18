using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientItemMoveFromSupplySatchel)]
    public class ClientItemMoveFromSupplySatchel : IReadable
    {
        public ushort MaterialId { get; private set; } // 14
        public uint Amount { get; private set; }
        public ushort Unknown2 { get; private set; } // 9
        public uint Unknown3 { get; private set; }

        public void Read(GamePacketReader reader)
        {
            MaterialId = reader.ReadUShort(14u);
            Amount = reader.ReadUInt();
            Unknown2 = reader.ReadUShort(9);
            Unknown3 = reader.ReadUInt();
        }
    }
}
