using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientWhoRequest)]
    public class ClientWhoRequest : IReadable
    {
        public void Read(GamePacketReader reader)
        {
        }
    }
}
