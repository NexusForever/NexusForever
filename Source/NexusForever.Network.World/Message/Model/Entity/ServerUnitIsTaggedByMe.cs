using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // Sent to player that tagged the unit.
    // Use ServerUnitTagOwner to notify other players of the tag ownership change.
    [Message(GameMessageOpcode.ServerUnitIsTaggedByMe)]
    public class ServerUnitIsTaggedByMe : IWritable
    {
        public uint UnitId { get; set; }
        public bool IsTaggedByMe { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(IsTaggedByMe);
        }
    }
}
