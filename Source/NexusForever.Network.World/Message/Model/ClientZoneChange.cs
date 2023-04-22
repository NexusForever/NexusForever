using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientZoneChange)]
    public class ClientZoneChange : IReadable
    {
        public ushort PreviousZoneId { get; private set; }
        public ushort NewZoneId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            PreviousZoneId = reader.ReadUShort(15u);
            NewZoneId = reader.ReadUShort(15u);
        }
    }
}
