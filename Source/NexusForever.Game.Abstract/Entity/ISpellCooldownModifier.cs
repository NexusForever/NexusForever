using NexusForever.Game.Abstract.Spell.Info;
using NexusForever.Game.Static.Spell.Effect;

namespace NexusForever.Game.Abstract.Entity
{
    public interface ISpellCooldownModifier
    {
        /// <summary>
        /// Id of the spell that applied the modifier.
        /// </summary>
        uint Spell4Id { get; }

        /// <summary>
        /// Id of the spell effect that applied the modifier.
        /// </summary>
        uint Spell4EffectId { get; }

        SpellEffectModifyCooldownScope Scope { get; }
        uint Key { get; }
        SpellEffectModifyCooldownMode Mode { get; }
        float Magnitude { get; }

        /// <summary>
        /// Return whether the modifier applies to the cooldown of the supplied <see cref="ISpellInfo"/>.
        /// </summary>
        bool Matches(ISpellInfo spellInfo);

        /// <summary>
        /// Apply the modifier to the supplied cooldown, both in seconds.
        /// </summary>
        /// <remarks>
        /// <paramref name="input"/> is the value <see cref="SpellEffectModifyCooldownMode.MultiplyClamped"/> multiplies
        /// and clamps against, <paramref name="accumulator"/> the value the remaining modes build on. They are the same
        /// value unless several modifiers are folded into one cooldown.
        /// </remarks>
        double Apply(double input, double accumulator);
    }
}
