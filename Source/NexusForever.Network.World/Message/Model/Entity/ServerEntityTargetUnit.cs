using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    /// <summary>
    /// Values from this packet are handled in the same client function as <see cref="ServerEntityThreatUpdate"/>.
    /// Additionally it will set the target id on the entity object in the client.
    /// </summary>
    [Message(GameMessageOpcode.ServerEntityTargetUnit)]
    public class ServerEntityTargetUnit : IWritable
    {
        public uint UnitId { get; set; }
        public uint NewTargetId { get; set; }
        public uint ThreatLevel { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(NewTargetId);
            writer.Write(ThreatLevel);
        }
    }
}
