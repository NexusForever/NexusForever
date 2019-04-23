using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerPlayerEnteredWorld)]
    public class ServerPlayerEnteredWorld : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
        }
    }
}
