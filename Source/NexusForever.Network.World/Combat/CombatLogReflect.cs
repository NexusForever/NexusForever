using NexusForever.Game.Static.Combat;
using NexusForever.Game.Static.Spell;

namespace NexusForever.Network.World.Combat
{
    public class CombatLogReflect : ICombatLog
    {
        public CombatLogType Type => CombatLogType.Reflect;

        public uint DamageAmount { get; set; }
        public uint RawDamage { get; set; }
        public uint Shield { get; set; }
        public uint Overkill { get; set; }
        public uint Glance { get; set; }
        public uint Absorption { get; set; }
        public bool Unknown0 { get; set; }
        public bool BKilled { get; set; }
        public DamageType DamageType { get; set; } // 3u
        public SpellEffectType EffectType { get; set; } // 8u
        public CombatLogCastData CastData { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(DamageAmount);
            writer.Write(RawDamage);
            writer.Write(Shield);
            writer.Write(Overkill);
            writer.Write(Glance);
            writer.Write(Absorption);
            writer.Write(Unknown0);
            writer.Write(BKilled);
            writer.Write(DamageType, 3u);
            writer.Write(EffectType, 8u);
            CastData.Write(writer);
        }
    }
}
