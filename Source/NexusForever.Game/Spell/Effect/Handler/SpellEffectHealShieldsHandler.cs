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
    [SpellEffectHandler(SpellEffectType.HealShields)]
    public class SpellEffectHealShieldsHandler : ISpellEffectApplyHandler<ISpellEffectDefaultData>
    {
        #region Dependency Injection

        private readonly IFactory<IDamageCalculator> damageCalculatorFactory;

        public SpellEffectHealShieldsHandler(
            IFactory<IDamageCalculator> damageCalculatorFactory)
        {
            this.damageCalculatorFactory = damageCalculatorFactory;
        }

        #endregion

        /// <summary>
        /// Handle <see cref="ISpell"/> effect apply on <see cref="IUnitEntity"/> target.
        /// </summary>
        /// <remarks>
        /// <see cref="SpellEffectType.HealShields"/> restores shield capacity rather than health. It shares
        /// the amount case of <see cref="SpellEffectType.Heal"/> and the same combat log record; see
        /// <see cref="SpellEffectHealHandler"/> for why the damage path is not used.
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

            // the setter clamps to MaxShieldCapacity
            uint shieldBefore = target.Shield;
            target.Shield = shieldBefore + amount;
            uint healed = target.Shield - shieldBefore;

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
