using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientStorefrontRequestCatalog)]
    public class ClientStorefrontRequestCatalog : IReadable
    {
        public ushort Unknown0 { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Unknown0 = reader.ReadUShort(14u);
        }
    }
}
