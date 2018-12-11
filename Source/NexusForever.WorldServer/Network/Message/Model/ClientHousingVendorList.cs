using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientHousingVendorList, MessageDirection.Client)]
    public class ClientHousingVendorList : IReadable
    {
        public void Read(GamePacketReader reader)
        {
        }
    }
}
