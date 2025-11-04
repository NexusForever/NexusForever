using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerCinematicWhiteOut)]
    public class ServerCinematicWhiteOut : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
            // zero length message
        }
    }
}
