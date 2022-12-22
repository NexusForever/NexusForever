using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerStoreFinalise)]
    public class ServerStoreFinalise : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
        }
    }
}
