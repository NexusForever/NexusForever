using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientMatchingQueueLeaveAll)]
    public class ClientMatchingQueueLeaveAll : IReadable
    {
        public void Read(GamePacketReader reader)
        {
            // deliberately empty
        }
    }
}
