using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerUnitTagOwner)]
    public class ServerUnitTagOwner : IWritable
    {
        public uint TaggedUnitId { get; set; }
        public uint TagOwnerUnitId { get; set; } // Player UnitId that owns the unit's tag
        public ulong TagOwnerGroupId { get; set; } // GroupId of the player that owns the unit's tag

        public void Write(GamePacketWriter writer)
        {
            writer.Write(TaggedUnitId);
            writer.Write(TagOwnerUnitId);
            writer.Write(TagOwnerGroupId);
        }
    }
}
