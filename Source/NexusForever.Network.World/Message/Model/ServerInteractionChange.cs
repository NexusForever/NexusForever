using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // Switches interaction to the specified unit.
    // If this differs from the clients current InteractionUnitId,
    // changes the active InteractionType to none.
    [Message(GameMessageOpcode.ServerInteractionChange)]
    public class ServerInteractionChange : IWritable
    {
        public uint InteractionUnitId { get; set; } = 0;

        public void Write(GamePacketWriter writer)
        {
            writer.Write(InteractionUnitId);
        }
    }
}
