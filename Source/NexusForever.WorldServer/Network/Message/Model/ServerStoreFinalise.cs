using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerStoreFinalise)]
    public class ServerStoreFinalise : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
        }
    }
}
