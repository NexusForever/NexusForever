using System.Numerics;
using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Entity.Creature;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Abstract.Spell.Target;
using NexusForever.Game.Abstract.Spell.Validator;
using NexusForever.Game.Spell.Event;
using NexusForever.Game.Static.Spell;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model.Spell;
using NexusForever.Shared.Game;

namespace NexusForever.Game.Spell.Type
{
    public class SpellAura : Spell
    {
        public override CastMethod CastMethod => CastMethod.Aura;

        private readonly UpdateTimer auraExecute = new(TimeSpan.FromMilliseconds(100));
        private readonly Dictionary<Spell4EffectsEntry, UpdateTimer> effectRetriggerTimers = [];

        #region Dependency Injection

        private readonly ILogger<SpellAura> log;
        private readonly ISpellTargetInfoCollection spellTargetInfoCollection;
        private readonly ICreatureInfoManager creatureInfoManager;

        public SpellAura(
            ILogger<SpellAura> log,
            ISpellTargetInfoCollection spellTargetInfoCollection,
            ICreatureInfoManager creatureInfoManager,
            IGlobalSpellManager globalSpellManager,
            ICastResultValidatorManager castResultValidatorManager,
            IDisableManager disableManager)
            : base(log, spellTargetInfoCollection, globalSpellManager, castResultValidatorManager, disableManager)
        {
            this.log                       = log;
            this.spellTargetInfoCollection = spellTargetInfoCollection;
            this.creatureInfoManager       = creatureInfoManager;
        }

        #endregion

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public override void Update(double lastTick)
        {
            base.Update(lastTick);

            if (status != SpellStatus.Executing)
                return;

            HandleOutOfRangeTargets(lastTick);
            HandleEffectTicks(lastTick);
        }

        private void HandleOutOfRangeTargets(double lastTick)
        {
            auraExecute.Update(lastTick);
            if (!auraExecute.HasElapsed)
                return;

            var executionContext = new SpellExecutionContext();
            executionContext.Initialise(this);

            SelectTargets(executionContext);

            foreach (ISpellTargetInfo targetInfo in spellTargetInfoCollection)
            {
                ISpellTarget target = executionContext.TargetCollection.GetTarget(targetInfo.Guid, SpellEffectTargetFlags.ImplicitTarget);
                if (target == null)
                    targetInfo.Finish();
            }

            auraExecute.Reset();
        }

        private void HandleEffectTicks(double lastTick)
        {
            var executionContext = new SpellExecutionContext();
            executionContext.Initialise(this);

            foreach ((Spell4EffectsEntry effect, UpdateTimer updateTimer) in effectRetriggerTimers)
            {
                updateTimer.Update(lastTick);
                if (updateTimer.HasElapsed)
                    executionContext.AddSpellEffect(effect);
            }

            Execute(executionContext);
        }

        public override bool Cast()
        {
            if (!base.Cast())
                return false;

            uint castTime = Parameters.CastTimeOverride > -1 ? (uint)Parameters.CastTimeOverride : Parameters.SpellInfo.Entry.CastTime;
            events.EnqueueEvent(new SpellEvent(castTime / 1000d, () =>
            {
                SpellStatus previousStatus = status;
                if ((currentPhase == 0 || currentPhase == 255) && previousStatus != SpellStatus.Executing)
                {
                    CostSpell();
                    SetCooldown();
                }
                Execute(false);
            })); // enqueue spell to be executed after cast time

            foreach (Spell4EffectsEntry effect in Parameters.SpellInfo.Effects.Where(i => i.TickTime > 0))
                effectRetriggerTimers.Add(effect, new UpdateTimer(TimeSpan.FromMilliseconds(effect.TickTime)));

            if (Parameters.SpellInfo.Entry.SpellDuration > 0 && Parameters.SpellInfo.Entry.SpellDuration < uint.MaxValue)
                events.EnqueueEvent(new SpellEvent(Parameters.SpellInfo.Entry.SpellDuration / 1000d, Finish));

            if (Parameters.SpellInfo.BaseInfo.Entry.Creature2IdPositionalAoe > 0)
            {
                ICreatureInfo creatureInfo = creatureInfoManager.GetCreatureInfo(Parameters.SpellInfo.BaseInfo.Entry.Creature2IdPositionalAoe);
                if (creatureInfo == null)
                    return false;

                Caster.SummonFactory.Summon(creatureInfo, Caster.Position, Caster.Rotation, (IBaseMap map, uint guid, Vector3 vector) =>
                {
                    Parameters.PositionalUnitId = guid;

                    status = SpellStatus.Casting;
                    log.LogTrace($"Spell {Parameters.SpellInfo.Entry.Id} has started casting.");
                });
            }
            else
            {
                status = SpellStatus.Casting;
                log.LogTrace($"Spell {Parameters.SpellInfo.Entry.Id} has started casting.");
            }
            return true;
        }

        protected override bool _IsCasting()
        {
            return base._IsCasting() && status == SpellStatus.Casting;
        }

        protected override bool CanExecuteEffect(Spell4EffectsEntry spell4EffectsEntry)
        {
            if (effectRetriggerTimers.TryGetValue(spell4EffectsEntry, out UpdateTimer updateTimer)
                && !updateTimer.HasElapsed)
                return false;

            return base.CanExecuteEffect(spell4EffectsEntry);
        }

        protected override void ExecuteEffect(Spell4EffectsEntry spell4EffectsEntry, ISpellExecutionContext executionContext)
        {
            base.ExecuteEffect(spell4EffectsEntry, executionContext);

            if (effectRetriggerTimers.TryGetValue(spell4EffectsEntry, out UpdateTimer updateTimer))
                updateTimer.Reset();
        }

        // TODO: research more, when should this be sent?
        private void SendBuffsApplied(List<uint> unitIds)
        {
            if (unitIds.Count == 0)
                return;

            var serverSpellBuffsApply = new ServerSpellBuffsUpdate();
            foreach (uint unitId in unitIds)
            {
                serverSpellBuffsApply.Buffs.Add(new ServerSpellBuffsUpdate.BuffInfo
                {
                    ServerUniqueId = CastingId,
                    TargetUnitId   = unitId,
                    BuffCount      = 1 // TODO: If something stacks, we may need to grab this from the target unit
                });
            }

            Caster.EnqueueToVisible(serverSpellBuffsApply, true);
        }

        // TODO: research more, when should this be sent?
        private void SendRemoveBuff(uint unitId)
        {
            if (!Parameters.SpellInfo.BaseInfo.HasIcon)
                throw new InvalidOperationException();

            Caster.EnqueueToVisible(new ServerSpellBuffRemoveSingle
            {
                ServerUniqueId = CastingId,
                TargetUnitId   = unitId
            }, true);
        }
    }
}
