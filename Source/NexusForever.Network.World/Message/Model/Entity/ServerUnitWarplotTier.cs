using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Entity
{
    // Only used on Units of StructuredPlug type in warplots
    [Message(GameMessageOpcode.ServerUnitWarplotTier)]
    public class ServerUnitWarplotTier : IWritable
    {
        public uint UnitId { get; set; }
        public uint WarplotTier { get; set; } // zero indexed

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(WarplotTier);
        }
    }
}
