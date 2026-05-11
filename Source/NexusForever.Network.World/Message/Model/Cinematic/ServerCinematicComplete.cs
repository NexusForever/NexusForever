using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Cinematic
{
    [Message(GameMessageOpcode.ServerCinematicComplete)]
    public class ServerCinematicComplete : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
            // zero byte message
        }
    }
}
