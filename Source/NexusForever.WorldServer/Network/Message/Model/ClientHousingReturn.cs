using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientHousingReturn)]
    public class ClientHousingReturn : IReadable
    {
        public void Read(GamePacketReader reader)
        {
        }
    }
}
