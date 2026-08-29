using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Instance
{
    // Can only be sent if a ServerPendingWorldRemoval (0x0689) has been sent and is still active
    [Message(GameMessageOpcode.ClientLeavePendingRemovalInstance)]
    public class ClientLeavePendingRemovalInstance : IReadable
    {
        public void Read(GamePacketReader reader)
        {
            // Zero byte message
        }
    }
}
