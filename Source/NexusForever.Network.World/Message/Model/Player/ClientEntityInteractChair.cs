using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Player
{
    [Message(GameMessageOpcode.ClientEntityInteractChair)]
    public class ClientEntityInteractChair : IReadable
    {
        public uint ChairUnitId { get; private set; }
        public bool Remove { get; private set; } // always 0

        public void Read(GamePacketReader reader)
        {
            ChairUnitId = reader.ReadUInt();
            Remove = reader.ReadBit();
        }
    }
}
