using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientMatchingStopLookingForReplacements)]
    public class ClientMatchingStopLookingForReplacements : IReadable
    {
        public void Read(GamePacketReader reader)
        {
            // Zero byte message
        }
    }
}
