using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientGroupLeave)]
    public class ClientGroupLeave : IReadable
    {
        public ulong GroupId { get; private set; }
        public bool Disband { get; private set; } // 0 = leave group, 1 = disband group

        public void Read(GamePacketReader reader)
        {
            GroupId = reader.ReadULong();
            Disband = reader.ReadBit();
        }
    }
}
