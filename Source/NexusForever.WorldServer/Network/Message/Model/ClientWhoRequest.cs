using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientWhoRequest, MessageDirection.Client)]
    public class ClientWhoRequest : IReadable
    {
        public void Read(GamePacketReader reader)
        {
        }
    }
}
