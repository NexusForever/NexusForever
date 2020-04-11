using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerCinematicComplete)]
    public class ServerCinematicComplete : IWritable
    {

        public void Write(GamePacketWriter writer)
        {
        }
    }
}
