using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{

    [Message(GameMessageOpcode.ServerPathRefresh)]
    public class ServerPathRefresh : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
        }
    }
}
