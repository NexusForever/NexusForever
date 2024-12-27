using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerMatchingMatchExited)]
    public class ServerMatchingMatchExited : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
        }
    }
}
