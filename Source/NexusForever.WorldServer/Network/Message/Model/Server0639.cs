using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.Server0639)]
    public class Server0639 : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
        }
    }
}
