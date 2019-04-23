using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
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
