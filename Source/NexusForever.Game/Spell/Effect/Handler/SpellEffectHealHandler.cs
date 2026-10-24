using NexusForever.Game.Abstract.Combat;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Abstract.Spell.Effect;
using NexusForever.Game.Abstract.Spell.Effect.Data;
using NexusForever.Game.Abstract.Spell.Target;
using NexusForever.Game.Static.Spell;
using NexusForever.Game.Static.Spell.Effect;
using NexusForever.Network.World.Combat;
using NexusForever.Shared;

namespace NexusForever.Game.Spell.Effect.Handler
{
    [SpellEffectHandler(SpellEffectType.Heal)]
    public class SpellEffectHealHandler : ISpellEffectApplyHandler<ISpellEffectDefaultData>
    {
        #region Dependency Injection

        private readonly IFactory<IDamageCalculator> damageCalculatorFactory;

        public SpellEffectHealHandler(
            IFactory<IDamageCalculator> damageCalculatorFactory)
        {
            this.damageCalculatorFactory = damageCalculatorFactory;
        }

        #endregion

        /// <summary>
        /// Handle <see cref="ISpell"/> effect apply on <see cref="IUnitEntity"/> target.
        /// </summary>
        /// <remarks>
        /// This deliberately does not go through <see cref="IDamageCalculator.CalculateDamage"/> or
        /// <see cref="IUnitEntity.TakeDamage"/>. 91.77% of the 3,098 shipped <see cref="SpellEffectType.Heal"/>
        /// rows carry damageType 0, and <see cref="DamageType.Heal"/> is 4, so the damage path would
        /// subtract health, mitigate the heal by armour and shields and refuse the effect outright on
        /// <see cref="IUnitEntity.CanAttack"/>.
        /// </remarks>
        public SpellEffectExecutionResult Apply(ISpellExecutionContext executionContext, IUnitEntity target, ISpellTargetEffectInfo info, ISpellEffectDefaultData data)
        {
            if (!target.IsAlive)
                return SpellEffectExecutionResult.PreventEffect;

            IDamageCalculator damageCalculator = damageCalculatorFactory.Resolve();
            damageCalculator.CalculateHeal(executionContext, target, info);

            if (info.Damage == null)
                return SpellEffectExecutionResult.Ok;

            uint amount = info.Damage.AdjustedDamage;

            uint healthBefore = target.Health;
            target.ModifyHealth(amount, DamageType.Heal, executionContext.Spell.Caster);
            uint healed = target.Health - healthBefore;

            executionContext.AddCombatLog(new CombatLogHeal
            {
                HealAmount = healed,
                Overheal   = amount - healed,
                // third u32 of the shared heal record; the client reads it but nothing here fills it
                Unknown0   = 0,
                EffectType = info.Entry.EffectType,
                CastData   = new CombatLogCastData
                {
                    CasterId     = executionContext.Spell.Caster.Guid,
                    TargetId     = target.Guid,
                    SpellId      = executionContext.Spell.Parameters.SpellInfo.Entry.Id,
                    CombatResult = info.Damage.CombatResult
                }
            });

            return SpellEffectExecutionResult.Ok;
        }
    }
}
