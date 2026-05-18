using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientMatchingMatchLeave)]
    public class ClientMatchingMatchLeave : IReadable
    {
        public void Read(GamePacketReader reader)
        {
            // deliberately empty
        }
    }
}
