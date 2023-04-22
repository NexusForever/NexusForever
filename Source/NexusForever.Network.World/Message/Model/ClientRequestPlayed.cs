using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientRequestPlayed)]
    public class ClientRequestPlayed : IReadable
    {
        public void Read(GamePacketReader reader)
        {
            // packet has no payload
        }
    }
}
