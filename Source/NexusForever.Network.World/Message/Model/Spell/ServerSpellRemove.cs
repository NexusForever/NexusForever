using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Spell
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
        public List<TargetSpellDodgeResult> TargetDodgeResults { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CastingId);
            writer.Write(TargetDodgeResults.Count, 32u);
            TargetDodgeResults.ForEach(u => u.Write(writer));
        }
    }
}
