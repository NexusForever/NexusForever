using NexusForever.Network.Message;
using NexusForever.Network.World.Entity;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientMoveToCast)]
    public class ClientMoveToCast : IReadable
    {
        public uint ClientSpellCastUniqueId { get; private set; }
        public ushort BagIndex { get; private set; }
        public Position MoveToPosition { get; private set; } = new Position();

        public void Read(GamePacketReader reader)
        {
            ClientSpellCastUniqueId = reader.ReadUInt();
            BagIndex = reader.ReadUShort();
            MoveToPosition.Read(reader);
        }
    }
}
