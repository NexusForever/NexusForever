using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPlayerEnteredWorld)]
    public class ServerPlayerEnteredWorld : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
        }
    }
}
