using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{

    [Message(GameMessageOpcode.ServerPathRefresh, MessageDirection.Server)]
    public class ServerPathRefresh : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
        }
    }
}
