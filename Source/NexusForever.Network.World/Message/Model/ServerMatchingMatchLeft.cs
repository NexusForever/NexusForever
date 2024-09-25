using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerMatchingMatchLeft)]
    public class ServerMatchingMatchLeft : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
            writer.Write(0, 5u);
        }
    }
}
