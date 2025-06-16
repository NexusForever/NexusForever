using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // Removes movement control from the client. Stops the client from sending movement updates.
    [Message(GameMessageOpcode.ServerMovementControlRemove)]
    public class ServerMovementControlRemove : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
        }
    }
}
