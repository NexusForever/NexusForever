using NexusForever.Game.Static.Spell;

namespace NexusForever.Game.Abstract.Spell
{
    public interface IDamageDescription
    {
        DamageType DamageType { get; set; }
        uint RawDamage { get; set; }
        uint RawScaledDamage { get; set; }
        uint AbsorbedAmount { get; set; }
        uint ShieldAbsorbAmount { get; set; }
        uint AdjustedDamage { get; set; }
        uint OverkillAmount { get; set; }
        bool KilledTarget { get; set; }
        CombatResult CombatResult { get; set; }
    }
}