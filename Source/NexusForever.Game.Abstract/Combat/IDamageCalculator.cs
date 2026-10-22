using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Abstract.Spell.Target;

namespace NexusForever.Game.Abstract.Combat
{
    public interface IDamageCalculator
    {
        /// <summary>
        /// Returns the calculated damage and updates the referenced <see cref="ISpellTargetEffectInfo"/> appropriately.
        /// </summary>
        /// <remarks>
        /// TODO: This should probably return an instance of a Class which describes all the damage done to both entities. Attackers can have reflected damage from this, etc.
        /// </remarks>
        void CalculateDamage(ISpellExecutionContext executionContext, IUnitEntity victim, ISpellTargetEffectInfo info);

        /// <summary>
        /// Calculates the healing amount and updates the referenced <see cref="ISpellTargetEffectInfo"/>
        /// appropriately.
        /// </summary>
        /// <remarks>
        /// Shares the amount arithmetic with <see cref="CalculateDamage"/> but applies none of the
        /// attack-side steps: no deflect, no armour mitigation and no shield absorption.
        /// </remarks>
        void CalculateHeal(ISpellExecutionContext executionContext, IUnitEntity target, ISpellTargetEffectInfo info);
    }
}
