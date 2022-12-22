using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientEntityInteractChair)]
    public class ClientEntityInteractChair : IReadable
    {
        public uint ChairUnitId { get; private set; }
        public bool Remove { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ChairUnitId = reader.ReadUInt();
            Remove      = reader.ReadBit();
        }
    }
}
