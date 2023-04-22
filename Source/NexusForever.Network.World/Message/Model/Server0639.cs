using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.Server0639)]
    public class Server0639 : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
        }
    }
}
