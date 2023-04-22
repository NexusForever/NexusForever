using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientWhoRequest)]
    public class ClientWhoRequest : IReadable
    {
        public void Read(GamePacketReader reader)
        {
        }
    }
}
