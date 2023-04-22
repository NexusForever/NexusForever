using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerCinematicComplete)]
    public class ServerCinematicComplete : IWritable
    {

        public void Write(GamePacketWriter writer)
        {
        }
    }
}
