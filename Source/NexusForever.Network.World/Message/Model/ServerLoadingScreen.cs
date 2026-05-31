using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // Seems be to used after a cinematic
    [Message(GameMessageOpcode.ServerLoadingScreen)]
    public class ServerLoadingScreen : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
            // zero byte message
        }
    }
}
