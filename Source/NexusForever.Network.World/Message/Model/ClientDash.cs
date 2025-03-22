using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientDash)]
    public class ClientDash : IReadable
    {
        public uint DashDirection { get; set; }

        public void Read(GamePacketReader reader)
        {
            DashDirection = reader.ReadUInt(3);
        }
    }
}
