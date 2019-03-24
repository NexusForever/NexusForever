using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientRequestPlayed, MessageDirection.Client)]
    public class ClientRequestPlayed : IReadable
    {
        public void Read(GamePacketReader reader)
        {
            // packet has no payload
        }
    }
}
