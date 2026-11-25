using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell.Info;
using NexusForever.Game.Static.Spell.Effect;
using NLog;

namespace NexusForever.Game.Entity
{
    public class SpellCooldownModifier : ISpellCooldownModifier
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private static readonly HashSet<uint> loggedUnknownModes = new();

        public uint Spell4Id { get; }
        public uint Spell4EffectId { get; }
        public SpellEffectModifyCooldownScope Scope { get; }
        public uint Key { get; }
        public SpellEffectModifyCooldownMode Mode { get; }
        public float Magnitude { get; }

        public SpellCooldownModifier(uint spell4Id, uint spell4EffectId, SpellEffectModifyCooldownScope scope, uint key,
            SpellEffectModifyCooldownMode mode, float magnitude)
        {
            Spell4Id       = spell4Id;
            Spell4EffectId = spell4EffectId;
            Scope          = scope;
            Key            = key;
            Mode           = mode;
            Magnitude      = magnitude;
        }

        /// <summary>
        /// Return whether the modifier applies to the cooldown of the supplied <see cref="ISpellInfo"/>.
        /// </summary>
        public bool Matches(ISpellInfo spellInfo)
        {
            if (spellInfo == null || Key == 0u)
                return false;

            return Scope switch
            {
                SpellEffectModifyCooldownScope.BaseSpell       => spellInfo.Entry.Spell4BaseIdBaseSpell == Key,
                SpellEffectModifyCooldownScope.Spell4          => spellInfo.Entry.Id == Key,
                SpellEffectModifyCooldownScope.SpellGroupId    => spellInfo.SpellGroups.Contains(Key),
                SpellEffectModifyCooldownScope.SpellCooldownId => spellInfo.Entry.SpellCoolDownIds.Contains(Key),
                _                                              => false
            };
        }

        /// <summary>
        /// Apply the modifier to the supplied cooldown, both in seconds.
        /// </summary>
        public double Apply(double input, double accumulator)
        {
            int inputMs       = ToMilliseconds(input);
            int accumulatorMs = ToMilliseconds(accumulator);

            long result;
            switch (Mode)
            {
                // a multiplier that can only shorten; it multiplies the unmodified value rather than the accumulator
                case SpellEffectModifyCooldownMode.MultiplyClamped:
                    result = Math.Min((int)(inputMs * Magnitude), inputMs);
                    break;
                case SpellEffectModifyCooldownMode.Multiply:
                    result = (int)(accumulatorMs * Magnitude);
                    break;
                case SpellEffectModifyCooldownMode.AddMilliseconds:
                    result = accumulatorMs + (long)(int)Magnitude;
                    break;
                case SpellEffectModifyCooldownMode.SetMilliseconds:
                    result = (int)Magnitude;
                    break;
                default:
                    // one shipped row has a mode of 219; the client falls through its mode ladder and changes nothing
                    if (loggedUnknownModes.Add((uint)Mode))
                        log.Warn($"Spell effect {Spell4EffectId} of spell {Spell4Id} has unknown cooldown modifier mode {(uint)Mode}.");

                    return accumulator;
            }

            return Math.Clamp(result, 0L, int.MaxValue) / 1000d;
        }

        private static int ToMilliseconds(double cooldown)
        {
            return (int)Math.Clamp(Math.Round(cooldown * 1000d), 0d, int.MaxValue);
        }
    }
}
