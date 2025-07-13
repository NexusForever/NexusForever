using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerEntityDestroy)]
    public class ServerEntityDestroy : IWritable
    {
        public uint UnitId { get; set; }
        public bool UseDeathAnimation { get; set; } // Only applies to units of type Chest

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(UseDeathAnimation);
        }
    }
}
