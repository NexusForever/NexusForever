using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientGroupLeave)]
    public class ClientGroupLeave : IReadable
    {
        public ulong GroupId { get; set; }

        public bool ShouldDisband { get; set; }


        public void Read(GamePacketReader reader)
        {
            GroupId = reader.ReadULong();
            ShouldDisband = reader.ReadBit();
        }
    }
}
