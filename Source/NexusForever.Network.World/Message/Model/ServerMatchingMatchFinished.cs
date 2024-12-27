using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerMatchingMatchFinished)]
    public class ServerMatchingMatchFinished : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
        }
    }
}
