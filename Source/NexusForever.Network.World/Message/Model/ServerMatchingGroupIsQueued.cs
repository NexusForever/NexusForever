using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerMatchingGroupIsQueued)]
    public class ServerMatchingGroupIsQueued : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
            // Zero byte message
        }
    }
}
