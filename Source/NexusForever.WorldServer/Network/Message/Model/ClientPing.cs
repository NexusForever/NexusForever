using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientPing, MessageDirection.Client)]
    public class ClientPing : IReadable 
    {
        public void Read(GamePacketReader reader)
        {
        }
    }
}
