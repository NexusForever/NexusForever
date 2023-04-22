using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Spell;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Abstract.Spell
{
    public interface IGlobalSpellManager
    {
        /// <summary>
        /// Id to be assigned to the next spell cast.
        /// </summary>
        uint NextCastingId { get; }

        /// <summary>
        /// Id to be assigned to the next spell effect.
        /// </summary>
        uint NextEffectId { get; }

        void Initialise();

        /// <summary>
        /// Return all <see cref="Spell4Entry"/>'s for the supplied spell base id.
        /// </summary>
        /// <remarks>
        /// This should only be used for cache related code, if you want an overview of a spell use <see cref="ISpellBaseInfo"/>.
        /// </remarks>
        IEnumerable<Spell4Entry> GetSpell4Entries(uint spell4BaseId);

        /// <summary>
        /// Return all <see cref="Spell4EffectsEntry"/>'s for the supplied spell id.
        /// </summary>
        /// <remarks>
        /// This should only be used for cache related code, if you want an overview of a spell use <see cref="ISpellBaseInfo"/>.
        /// </remarks>
        IEnumerable<Spell4EffectsEntry> GetSpell4EffectEntries(uint spell4Id);

        /// <summary>
        /// Return all <see cref="TelegraphDamageEntry"/>'s for the supplied spell id.
        /// </summary>
        /// <remarks>
        /// This should only be used for cache related code, if you want an overview of a spell use <see cref="ISpellBaseInfo"/>.
        /// </remarks>
        IEnumerable<TelegraphDamageEntry> GetTelegraphDamageEntries(uint spell4Id);

        /// <summary>
        /// Return <see cref="ISpellBaseInfo"/>, if not already cached it will be generated before being returned.
        /// </summary>
        ISpellBaseInfo GetSpellBaseInfo(uint spell4BaseId);

        /// <summary>
        /// Return <see cref="SpellEffectDelegate"/> for supplied <see cref="SpellEffectType"/>.
        /// </summary>
        SpellEffectDelegate GetEffectHandler(SpellEffectType spellEffectType);
    }
}