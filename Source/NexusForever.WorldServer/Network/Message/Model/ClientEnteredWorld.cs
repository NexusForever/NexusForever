using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientEnteredWorld)]
    public class ClientEnteredWorld : IReadable
    {
        public ushort WorldZoneId { get; private set; } // 15

        public void Read(GamePacketReader reader)
        {
            WorldZoneId = reader.ReadUShort(15u);
        }
    }
}
