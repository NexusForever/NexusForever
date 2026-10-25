using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Abstract.Spell.Effect;
using NexusForever.Game.Abstract.Spell.Effect.Data;
using NexusForever.Game.Abstract.Spell.Target;
using NexusForever.Game.Entity;
using NexusForever.Game.Static.Spell;
using NexusForever.Game.Static.Spell.Effect;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Spell.Effect.Handler
{
    [SpellEffectHandler(SpellEffectType.ModifySpellCooldown)]
    public class SpellEffectModifySpellCooldownHandler : ISpellEffectApplyHandler<ISpellEffectModifySpellCooldownData>, ISpellEffectRemoveHandler<ISpellEffectModifySpellCooldownData>
    {
        /// <summary>
        /// Handle <see cref="ISpell"/> effect apply on <see cref="IUnitEntity"/> target.
        /// </summary>
        public SpellEffectExecutionResult Apply(ISpellExecutionContext executionContext, IUnitEntity target, ISpellTargetEffectInfo info, ISpellEffectModifySpellCooldownData data)
        {
            // only players have cooldown state
            if (target is not IPlayer player)
                return SpellEffectExecutionResult.PreventEffect;

            // 16 shipped rows have no key and can never match a cooldown
            if (data.Key == 0u)
                return SpellEffectExecutionResult.PreventEffect;

            var modifier = new SpellCooldownModifier(executionContext.Spell.Parameters.SpellInfo.Entry.Id, info.Entry.Id,
                data.Scope, data.Key, data.Mode, data.Magnitude);

            // the client applies the modifier to every matching cooldown that is already running, once, when the
            // effect is applied, and does not undo it when the effect is removed
            player.SpellManager.ApplySpellCooldownModifier(modifier);

            // an effect with a lifetime also keeps the modifier, and it is folded into every cooldown that starts
            // while it is held
            if (IsHeld(info.Entry))
                player.SpellManager.AddSpellCooldownModifier(info, modifier);

            return SpellEffectExecutionResult.Ok;
        }

        /// <summary>
        /// Handle <see cref="ISpell"/> effect remove on <see cref="IUnitEntity"/> target.
        /// </summary>
        public void Remove(ISpell spell, IUnitEntity target, ISpellTargetEffectInfo info, ISpellEffectModifySpellCooldownData data)
        {
            if (target is not IPlayer player)
                return;

            player.SpellManager.RemoveSpellCooldownModifier(info);
        }

        /// <summary>
        /// Return whether the modifier is held for the lifetime of the effect rather than applied once.
        /// </summary>
        /// <remarks>
        /// The client keeps the modifier when the effect has a non-zero lifetime, which is its delay plus its duration,
        /// or until it is cancelled when <see cref="SpellEffectFlags.CancelOnly"/> is set.
        /// </remarks>
        private static bool IsHeld(Spell4EffectsEntry entry)
        {
            return entry.Flags.HasFlag(SpellEffectFlags.CancelOnly) || entry.DelayTime + entry.DurationTime > 0u;
        }
    }
}
