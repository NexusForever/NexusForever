using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerSpellRemove)]
    public class ServerSpellRemove : IWritable
    {
        public class TargetSpellDodgeResult : IWritable
        {
            public uint TargetUnitId { get; set; }
            public byte SpellDodged { get; set; } = 0;
            public uint AdditionalSoundDelay { get; set; } = 0;

            public void Write(GamePacketWriter writer)
            {
                writer.Write(TargetUnitId);
                writer.Write(SpellDodged, 4u);
                writer.Write(AdditionalSoundDelay);
            }
        }
        public uint CastingId { get; set; }

        public List<TargetSpellDodgeResult> targetDodgeResults { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CastingId);
            writer.Write(targetDodgeResults.Count, 32u);
            targetDodgeResults.ForEach(u => u.Write(writer));
        }
    }
}
