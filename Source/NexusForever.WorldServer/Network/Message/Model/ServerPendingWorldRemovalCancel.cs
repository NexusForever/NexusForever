using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerPendingWorldRemovalCancel)]
    public class ServerPendingWorldRemovalCancel : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
        }
    }
}
