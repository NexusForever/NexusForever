using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Static.Spell;
using NexusForever.Network.World.Message.Model.Spell;

namespace NexusForever.Game.Spell
{
    public class DamageDescription : IDamageDescription
    {
        public uint RawDamage { get; set; }
        public uint RawScaledDamage { get; set; }
        public uint AbsorbedAmount { get; set; }
        public uint ShieldAbsorbAmount { get; set; }
        public uint AdjustedDamage { get; set; }
        public uint OverkillAmount { get; set; }
        public bool KilledTarget { get; set; }
        public CombatResult CombatResult { get; set; }
        public DamageType DamageType { get; set; }

        public SpellEffectResult Build()
        {
            return new SpellEffectResult
            {
                RawDamage          = RawDamage,
                RawScaledDamage    = RawScaledDamage,
                AbsorbedAmount     = AbsorbedAmount,
                ShieldAbsorbAmount = ShieldAbsorbAmount,
                AdjustedDamage     = AdjustedDamage,
                OverkillAmount     = OverkillAmount,
                KilledTarget       = KilledTarget,
                CombatResult       = CombatResult,
                DamageType         = DamageType
            };
        }
    }
}
