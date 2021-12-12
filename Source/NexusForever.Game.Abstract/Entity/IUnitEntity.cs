using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Static.Spell;

namespace NexusForever.Game.Abstract.Entity
{
    /// <summary>
    /// An <see cref="IUnitEntity"/> is an extension to <see cref="IWorldEntity"/> which can cast spells, be targed by spells and participate in combat.
    /// </summary>
    public interface IUnitEntity : IWorldEntity
    {
        float HitRadius { get; }

        /// <summary>
        /// Cast a <see cref="ISpell"/> with the supplied spell id and <see cref="ISpellParameters"/>.
        /// </summary>
        void CastSpell(uint spell4Id, ISpellParameters parameters);

        /// <summary>
        /// Cast a <see cref="ISpell"/> with the supplied spell base id, tier and <see cref="ISpellParameters"/>.
        /// </summary>
        void CastSpell(uint spell4BaseId, byte tier, ISpellParameters parameters);

        /// <summary>
        /// Cast a <see cref="ISpell"/> with the supplied <see cref="ISpellParameters"/>.
        /// </summary>
        void CastSpell(ISpellParameters parameters);

        /// <summary>
        /// Cancel any <see cref="ISpell"/>'s that are interrupted by movement.
        /// </summary>
        void CancelSpellsOnMove();

        /// <summary>
        /// Cancel an <see cref="ISpell"/> based on its casting id.
        /// </summary>
        /// <param name="castingId">Casting ID of the spell to cancel</param>
        void CancelSpellCast(uint castingId);

        /// <summary>
        /// Checks if this <see cref="IUnitEntity"/> is currently casting a spell.
        /// </summary>
        /// <returns></returns>
        bool IsCasting();

        /// <summary>
        /// Check if this <see cref="IUnitEntity"/> has a spell active with the provided <see cref="Spell4Entry"/> Id
        /// </summary>
        bool HasSpell(uint spell4Id, out ISpell spell, bool isCasting = false);

        /// <summary>
        /// Check if this <see cref="IUnitEntity"/> has a spell active with the provided <see cref="CastMethod"/>
        /// </summary>
        bool HasSpell(CastMethod castMethod, out ISpell spell);

        /// <summary>
        /// Check if this <see cref="IUnitEntity"/> has a spell active with the provided <see cref="Func"/> predicate.
        /// </summary>
        bool HasSpell(Func<ISpell, bool> predicate, out ISpell spell);
    }
}