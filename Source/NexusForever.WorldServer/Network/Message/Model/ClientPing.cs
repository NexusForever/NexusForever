using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientPing)]
    public class ClientPing : IReadable 
    {
        public void Read(GamePacketReader reader)
        {
        }
    }
}
