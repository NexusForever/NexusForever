using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientDisembark, MessageDirection.Client)]
    public class ClientDisembark : IReadable
    {
        public void Read(GamePacketReader reader)
        {
            // packet has no payload
        }
    }
}
