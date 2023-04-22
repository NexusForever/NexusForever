using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerQueueFinish)]
    public class ServerQueueFinish : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
        }
    }
}
