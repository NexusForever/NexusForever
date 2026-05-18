using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientActivateUnit)]
    public class ClientActivateUnit : IReadable
    {
        public uint UnitId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            UnitId  = reader.ReadUInt();
        }
    }
}
