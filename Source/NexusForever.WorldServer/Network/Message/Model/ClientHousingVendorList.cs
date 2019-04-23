using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientHousingVendorList)]
    public class ClientHousingVendorList : IReadable
    {
        public void Read(GamePacketReader reader)
        {
        }
    }
}
