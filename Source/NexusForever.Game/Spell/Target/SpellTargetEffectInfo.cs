using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Abstract.Spell.Effect;
using NexusForever.Game.Abstract.Spell.Target;
using NexusForever.Game.Static.Spell;
using NexusForever.Game.Static.Spell.Effect;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model.Shared;
using NexusForever.Network.World.Message.Model.Spell;
using NexusForever.Shared.Game;

namespace NexusForever.Game.Spell.Target
{
    public class SpellTargetEffectInfo : ISpellTargetEffectInfo
    {
        /// <summary>
        /// Return whether the <see cref="ISpellTargetEffectInfo"/> has been finalised.
        /// </summary>
        /// <remarks>
        /// The effect will be finalised when the duration expires or the effect is prematurely finished.
        /// </remarks>
        public bool IsFinalised { get; private set; }

        /// <summary>
        /// Return whether the <see cref="ISpellTargetEffectInfo"/> is pending execution.
        /// </summary>
        /// <remarks>
        /// A pending effect has been sent to the client but has not yet been executed.
        /// </remarks>
        public bool IsPending { get; private set; }

        public ISpellTargetInfo Target { get; private set; }
        public uint EffectId { get; private set; }
        public Spell4EffectsEntry Entry { get; private set; }
        public IDamageDescription Damage { get; private set; }

        private UpdateTimer duration;

        #region Dependency Injection

        private ILogger<SpellTargetEffectInfo> log;
        private ISpellEffectHandlerInvoker spellEffectHandlerInvoker;
        private IGlobalSpellEffectManager globalSpellEffectManager;

        public SpellTargetEffectInfo(
            ILogger<SpellTargetEffectInfo> log,
            ISpellEffectHandlerInvoker spellEffectHandlerInvoker,
            IGlobalSpellEffectManager globalSpellEffectManager)
        {
            this.log                       = log;
            this.spellEffectHandlerInvoker = spellEffectHandlerInvoker;
            this.globalSpellEffectManager  = globalSpellEffectManager;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="ISpellTargetEffectInfo"/> with supplied <see cref="ISpellTargetInfo"/> and <see cref="Spell4EffectsEntry"/>.
        /// </summary>
        public void Initialise(ISpellTargetInfo target, Spell4EffectsEntry entry)
        {
            if (EffectId != 0)
                throw new InvalidOperationException("SpellTargetEffectInfo has already been initialised!");

            EffectId  = globalSpellEffectManager.NextEffectId;
            Target    = target;
            Entry     = entry;
            IsPending = entry.DelayTime > 0;

            if (!entry.Flags.HasFlag(SpellEffectFlags.CancelOnly))
                duration = new UpdateTimer(TimeSpan.FromMilliseconds(entry.DurationTime + entry.DelayTime));

            log.LogTrace($"Initalised effect {Entry.EffectType} for target {Target.Guid} for spell {Target.Collection.Spell.Spell4Id}");
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public void Update(double lastTick)
        {
            if (IsFinalised)
                return;

            if (IsPending)
                return;

            duration?.Update(lastTick);
            if (duration != null && duration.HasElapsed)
                Finish();
        }

        public EffectInfo Build()
        {
            var effectInfo = new EffectInfo
            {
                Spell4EffectId = Entry.Id,
                EffectUniqueId = EffectId,
                DelayTime      = Entry.DelayTime,
                TimeRemaining  = duration != null ? (int)TimeSpan.FromSeconds(duration.Duration).TotalMilliseconds : -1
            };

            if (Damage != null)
            {
                effectInfo.InfoType         = 1;
                effectInfo.EffectResultData = Damage.Build();
            }

            return effectInfo;
        }

        /// <summary>
        /// Execute the effect.
        /// </summary>
        /// <remarks>
        /// This will also invoke the <see cref="ISpellEffectApplyHandler"/> for the effect type if it exists.
        /// </remarks>
        public SpellEffectExecutionResult Execute(ISpellExecutionContext executionContext)
        {
            if (IsPending && !executionContext.IsDelayed)
                return SpellEffectExecutionResult.Pending;

            try
            {
                IUnitEntity target = Target.GetTarget();

                SpellEffectExecutionResult result = spellEffectHandlerInvoker.InvokeApplyHandler(executionContext, target, this);
                if (result == SpellEffectExecutionResult.Ok)
                {
                    if (executionContext.IsDelayed)
                    {
                        IsPending = false;

                        target.EnqueueToVisible(new ServerSpellEffectExecute()
                        {
                            CastingId           = executionContext.Spell.CastingId,
                            CasterUnitId        = target.Guid,
                            SpellEffectUniqueId = EffectId
                        }, true);
                    }
                }

                log.LogTrace($"Applied effect {Entry.EffectType} for target {Target.Guid} for spell {Target.Collection.Spell.Spell4Id}");

                return result;
            }
            catch (Exception e)
            {
                log.LogError(e, $"An exception occurred applying effect {Entry.EffectType} for target {Target.Guid} for spell {Target.Collection.Spell.Spell4Id}!");
                return SpellEffectExecutionResult.NoHandler;
            }
        }

        /// <summary>
        /// Finish the effect.
        /// </summary>
        /// <remarks>
        /// This will also invoke the <see cref="ISpellEffectRemoveHandler"/> for the effect type if it exists.
        /// </remarks>
        public void Finish()
        {
            if (IsFinalised)
                return;

            HandleSpellEffectRemove();
            IsFinalised = true;

            log.LogTrace($"Finished effect {Entry.EffectType} for target {Target.Guid} for spell {Target.Collection.Spell.Spell4Id}");
        }

        private void HandleSpellEffectRemove()
        {
            // don't execute remove effect if apply effect was not executed
            if (IsPending)
                return;

            try
            {
                spellEffectHandlerInvoker.InvokeRemoveHandler(Target.Collection.Spell, Target.GetTarget(), this);
            }
            catch (Exception e)
            {
                log.LogError(e, $"An exception occurred removing effect {Entry.EffectType} for target {Target.Guid} for spell {Target.Collection.Spell.Spell4Id}!");
            }

            log.LogTrace($"Removed effect {Entry.EffectType} for target {Target.Guid} for spell {Target.Collection.Spell.Spell4Id}");
        }

        /// <summary>
        /// Add damage to the effect.
        /// </summary>
        public void AddDamage(IDamageDescription damage)
        {
            Damage = damage;
            // TODO: what about delayed effects, is there a message we should send?
        }

        /// <summary>
        /// Return the effect duration.
        /// </summary>
        public TimeSpan? GetDuration()
        {
            return duration != null ? TimeSpan.FromSeconds(duration.Duration) : null;
        }

        /// <summary>
        /// Set the effect duration.
        /// </summary>
        /// <remarks>
        /// This will overwrite the default duration.
        /// </remarks>
        public void SetDuration(TimeSpan? timeSpan)
        {
            if (timeSpan != null)
                duration = new UpdateTimer(timeSpan.Value);
            else
                duration = null;

            // non delayed effects will send the updated duration in ServerSpellGo
            if (IsPending)
            {
                IUnitEntity target = Target.GetTarget();
                if (target == null)
                    return;

                target.EnqueueToVisible(new ServerSpellUpdateEffectDuration
                {
                    ServerUniqueId      = Target.Collection.Spell.CastingId,
                    TargetUnitId        = target.Guid,
                    SpellEffectUniqueId = EffectId,
                    TimeRemaining       = duration != null ? (int)TimeSpan.FromSeconds(duration.Duration).TotalMilliseconds : -1
                }, true);
            }
        }
    }
}
