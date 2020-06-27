using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
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
