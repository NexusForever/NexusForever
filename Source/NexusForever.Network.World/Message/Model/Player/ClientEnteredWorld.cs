using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Player
{
    [Message(GameMessageOpcode.ClientEnteredWorld)]
    public class ClientEnteredWorld : IReadable
    {
        public ushort WorldZoneId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            WorldZoneId = reader.ReadUShort(15u);
        }
    }
}
