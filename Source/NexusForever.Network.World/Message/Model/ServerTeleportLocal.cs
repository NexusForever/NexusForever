using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerTeleportLocal)]
    public class ServerTeleportLocal : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
            // deliberately empty
        }
    }
}
