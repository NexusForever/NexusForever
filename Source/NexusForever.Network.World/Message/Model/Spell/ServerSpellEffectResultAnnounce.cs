using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Spell
{
    [Message(GameMessageOpcode.ServerSpellEffectResultAnnounce)]
    public class ServerSpellEffectResultAnnounce : IWritable
    {
        public uint CastingId { get; set; }
        public uint Spell4EffectId { get; set; }
        public uint TargetUnitId { get; set; }

        public List<SpellEffectResult> SpellEffectResults { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CastingId);
            writer.Write(Spell4EffectId, 19);
            writer.Write(TargetUnitId);

            writer.Write(SpellEffectResults.Count, 8u);
            SpellEffectResults.ForEach(u => u.Write(writer));
        }
    }
}
