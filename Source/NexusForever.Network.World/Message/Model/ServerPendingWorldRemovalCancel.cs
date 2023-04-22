using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPendingWorldRemovalCancel)]
    public class ServerPendingWorldRemovalCancel : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
        }
    }
}
