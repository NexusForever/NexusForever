using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientHousingVendorList)]
    public class ClientHousingVendorList : IReadable
    {
        public void Read(GamePacketReader reader)
        {
        }
    }
}
