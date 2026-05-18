using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Utility
{
    [Message(GameMessageOpcode.ClientPlayedRequest)]
    public class ClientPlayedReqeust : IReadable
    {
        public void Read(GamePacketReader reader)
        {
            // packet has no payload
        }
    }
}
