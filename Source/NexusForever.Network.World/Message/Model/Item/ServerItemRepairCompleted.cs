using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Item
{
    // Triggers RepairItemCompleted UI event. Carbine UI does not have any event handlers for this message
    // and the message is not seen in sniffs, but could be used by custom UIs and addons if desired.
    [Message(GameMessageOpcode.ServerItemRepairCompleted)]
    public class ServerItemRepairCompleted : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
            // Zero byte message
        }
    }
}
