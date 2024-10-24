using NexusForever.Game.Static.Combat;
using NexusForever.Game.Static.Spell;

namespace NexusForever.Network.World.Combat
{
    public class CombatLogDamage : ICombatLog
    {
        public virtual CombatLogType Type => CombatLogType.Damage;

        public uint MitigatedDamage { get; set; }
        public uint RawDamage { get; set; }
        public uint Shield { get; set; }
        public uint Absorption { get; set; }
        public uint Overkill { get; set; }
        public uint Glance { get; set; }
        public bool BTargetVulnerable { get; set; }
        public bool BKilled { get; set; }
        public bool BPeriodic { get; set; }
        public DamageType DamageType { get; set; } // 3u
        public SpellEffectType EffectType { get; set; } // 8u
        public CombatLogCastData CastData { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(MitigatedDamage);
            writer.Write(RawDamage);
            writer.Write(Shield);
            writer.Write(Absorption);
            writer.Write(Overkill);
            writer.Write(Glance);
            writer.Write(BTargetVulnerable);
            writer.Write(BKilled);
            writer.Write(BPeriodic);
            writer.Write(DamageType, 3u);
            writer.Write(EffectType, 8u);
            CastData.Write(writer);
        }
    }
}
