using NexusForever.Game.Static.Spell;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Spell;

namespace NexusForever.Game.Abstract.Spell
{
    public interface IDamageDescription : INetworkBuildable<SpellEffectResult>
    {
        uint RawDamage { get; set; }
        uint RawScaledDamage { get; set; }
        uint AbsorbedAmount { get; set; }
        uint ShieldAbsorbAmount { get; set; }
        uint AdjustedDamage { get; set; }
        uint OverkillAmount { get; set; }
        bool KilledTarget { get; set; }
        CombatResult CombatResult { get; set; }
        DamageType DamageType { get; set; }
    }
}
