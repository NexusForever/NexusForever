using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Abstract.Spell.Effect;
using NexusForever.Game.Abstract.Spell.Effect.Data;
using NexusForever.Game.Abstract.Spell.Target;
using NexusForever.Game.Static.Spell;
using NexusForever.Game.Static.Spell.Effect;

namespace NexusForever.Game.Spell.Effect.Handler
{
    [SpellEffectHandler(SpellEffectType.PersonalDmgHealMod)]
    public class SpellEffectPersonalDmgHealModHandler : ISpellEffectApplyHandler<ISpellEffectPersonalDmgHealModData>, ISpellEffectRemoveHandler<ISpellEffectPersonalDmgHealModData>
    {
        /// <summary>
        /// Kinds this handler has already reported as unimplemented, so each is logged once per process.
        /// Concurrent because maps update on their own threads and this handler is transient.
        /// </summary>
        private static readonly ConcurrentDictionary<uint, byte> loggedKinds = [];

        #region Dependency Injection

        private readonly ILogger<SpellEffectPersonalDmgHealModHandler> log;

        public SpellEffectPersonalDmgHealModHandler(
            ILogger<SpellEffectPersonalDmgHealModHandler> log)
        {
            this.log = log;
        }

        #endregion

        /// <summary>
        /// Handle <see cref="ISpell"/> effect apply on <see cref="IUnitEntity"/> target.
        /// </summary>
        /// <remarks>
        /// A personal modifier applies to the pair (caster, target) rather than to the target as a whole,
        /// so it is stored against the caster's guid instead of going through
        /// <see cref="IUnitEntity.AddSpellModifierProperty"/>. Only
        /// <see cref="PersonalDmgHealModKind.HealingFromCaster"/> is consumed today, in
        /// <see cref="Combat.DamageCalculator.CalculateHeal"/>.
        ///
        /// Lifetime needs nothing here: the effect's duration is already handled by
        /// <see cref="ISpellTargetEffectInfo"/>, which builds no timer at all when
        /// <see cref="SpellEffectFlags.CancelOnly"/> is set. That matches the client exactly - its row
        /// validator requires DurationTime of at least 200ms unless that flag is set, and the 145 shipped
        /// rows split 67 (flag set, DurationTime 0) / 78 (flag clear, DurationTime 3,000-12,000).
        /// </remarks>
        public SpellEffectExecutionResult Apply(ISpellExecutionContext executionContext, IUnitEntity target, ISpellTargetEffectInfo info, ISpellEffectPersonalDmgHealModData data)
        {
            // the two rules the client's validator enforces for this effect type, in its own order.
            // neither fires on any shipped row; they only guard against edited or future data.
            if (!info.Entry.Flags.HasFlag(SpellEffectFlags.CancelOnly) && info.Entry.DurationTime < 200u)
            {
                log.LogWarning($"PersonalDmgHealMod effect {info.Entry.Id} has a duration of {info.Entry.DurationTime}ms and is not cancel only, ignoring!");
                return SpellEffectExecutionResult.PreventEffect;
            }

            if (data.Mode == 0u)
            {
                log.LogWarning($"PersonalDmgHealMod effect {info.Entry.Id} has no mode, ignoring!");
                return SpellEffectExecutionResult.PreventEffect;
            }

            if (data.Kind != (uint)PersonalDmgHealModKind.HealingFromCaster)
            {
                if (loggedKinds.TryAdd(data.Kind, 0))
                    log.LogWarning($"PersonalDmgHealMod kind {data.Kind} not implemented!");
            }
            else if (data.Mode != 1u)
            {
                // every shipped kind 12 row uses mode 1 and what mode 2 would change is unknown.
                log.LogWarning($"PersonalDmgHealMod effect {info.Entry.Id} is kind {data.Kind} with mode {data.Mode}, not applying!");
                return SpellEffectExecutionResult.PreventEffect;
            }

            target.AddPersonalSpellModifier(executionContext.Spell.Caster.Guid, data.Kind,
                executionContext.Spell.Parameters.SpellInfo.Entry.Id, data.Magnitude);

            return SpellEffectExecutionResult.Ok;
        }

        /// <summary>
        /// Handle <see cref="ISpell"/> effect remove on <see cref="IUnitEntity"/> target.
        /// </summary>
        public void Remove(ISpell spell, IUnitEntity target, ISpellTargetEffectInfo info, ISpellEffectPersonalDmgHealModData data)
        {
            target.RemovePersonalSpellModifier(spell.Caster.Guid, data.Kind, spell.Parameters.SpellInfo.Entry.Id);
        }
    }
}
