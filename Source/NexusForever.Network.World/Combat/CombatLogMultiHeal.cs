using NexusForever.Game.Static.Combat;
using NexusForever.Game.Static.Spell;

namespace NexusForever.Network.World.Combat
{
    public class CombatLogMultiHeal : ICombatLog
    {
        public CombatLogType Type => CombatLogType.MultiHeal;

        public uint HealAmount { get; set; }
        public uint Overheal { get; set; }
        public uint Absorption { get; set; }
        public SpellEffectType EffectType { get; set; } // 8u
        public CombatLogCastData CastData { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(HealAmount);
            writer.Write(Overheal);
            writer.Write(Absorption);
            writer.Write(EffectType, 8u);
            CastData.Write(writer);
        }
    }
}
