using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerMovementControlRemove)]
    public class ServerMovementControlRemove : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
        }
    }
}
