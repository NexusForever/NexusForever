using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerPlayerEnteredWorld, MessageDirection.Server)]
    public class ServerPlayerEnteredWorld : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
        }
    }
}
