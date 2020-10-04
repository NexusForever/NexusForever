using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
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
