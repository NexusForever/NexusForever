using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Abstract.Spell.Target;
using NexusForever.Game.Static.Spell;
using NexusForever.Game.Static.Spell.Effect;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model.Shared;
using NexusForever.Network.World.Message.Model.Spell;
using NexusForever.Shared;

namespace NexusForever.Game.Spell.Target
{
    public class SpellTargetInfo : ISpellTargetInfo
    {
        /// <summary>
        /// Return whether the <see cref="ISpellTargetInfo"/> has been finalised.
        /// </summary>
        /// <remarks>
        /// The target will be finalised when all effects are finialised.
        /// </remarks>
        public bool IsFinalised { get; private set; }

        public ISpellTargetInfoCollection Collection { get; private set; }
        public byte Index { get; private set; }
        public uint Guid { get; private set; }
        public SpellEffectTargetFlags Flags { get; private set; }

        private readonly Dictionary<uint, ISpellTargetEffectInfo> effects = [];

        #region Dependency Injection

        private readonly ILogger<SpellTargetInfo> log;
        private readonly IFactory<ISpellTargetEffectInfo> spellTargetEffectInfoFactory;

        public SpellTargetInfo(
            ILogger<SpellTargetInfo> log,
            IFactory<ISpellTargetEffectInfo> spellTargetEffectInfoFactory)
        {
            this.log                          = log;
            this.spellTargetEffectInfoFactory = spellTargetEffectInfoFactory;
        }

        #endregion

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public void Update(double lastTick)
        {
            if (IsFinalised)
                return;

            var toRemove = new List<ISpellTargetEffectInfo>();
            foreach (ISpellTargetEffectInfo effect in effects.Values)
            {
                effect.Update(lastTick);
                if (effect.IsFinalised)
                    toRemove.Add(effect);
            }

            RemoveFinalised(toRemove);
        }

        private void RemoveFinalised(List<ISpellTargetEffectInfo> toRemove)
        {
            foreach (ISpellTargetEffectInfo effect in toRemove)
            {
                effects.Remove(effect.Entry.Id);
                log.LogTrace($"Removed SpellTargetEffectInfo for target {Guid} for spell {Collection.Spell.Spell4Id}.");
            }

            if (effects.Count == 0)
            {
                SendBuffsRemoved([ Guid ]);

                IsFinalised = true;
                log.LogTrace($"Finalised SpellTargetInfo for target {Guid} for spell {Collection.Spell.Spell4Id}.");
            }
        }

        /// <summary>
        /// Initialise <see cref="ISpellTargetInfo"/> with supplied <see cref="ISpellTargetInfoCollection"/>, index and <see cref="ISpellTarget"/>.
        /// </summary>
        public void Initialise(ISpellTargetInfoCollection collection, byte index, ISpellTarget target)
        {
            if (Collection != null)
                throw new InvalidOperationException("SpellTargetInfo has already been initialised!");

            Collection = collection;
            Index      = index;
            Guid       = target.Entity.Guid;
            Flags      = target.Flags;

            log.LogTrace($"Initialised SpellTargetInfo for target {Guid} for spell {Collection.Spell.Spell4Id}.");
        }

        public TargetInfo Build()
        {
            var targetInfo = new TargetInfo
            {
                UnitId        = Guid,
                Ndx           = Index,
                TargetFlags   = Flags,
                InstanceCount = 1,
                CombatResult  = CombatResult.Hit
            };

            foreach (ISpellTargetEffectInfo targetEffectInfo in effects.Values)
                if (targetEffectInfo.Entry.EffectType != SpellEffectType.Proxy)
                    targetInfo.EffectInfoData.Add(targetEffectInfo.Build());

            return targetInfo;
        }

        /// <summary>
        /// Return any <see cref="ISpellTargetEffectInfo"/> for the target of the supplied <see cref="SpellEffectType"/>.
        /// </summary>
        public IEnumerable<ISpellTargetEffectInfo> GetEffectsByType(SpellEffectType type)
        {
            return effects.Values.Where(x => x.Entry.EffectType == type);
        }

        /// <summary>
        /// Create and execute the supplied <see cref="Spell4EffectsEntry"/>.
        /// </summary>
        public SpellEffectExecutionResult Execute(Spell4EffectsEntry entry, ISpellExecutionContext executionContext)
        {
            if (!effects.TryGetValue(entry.Id, out ISpellTargetEffectInfo info))
            {
                info = spellTargetEffectInfoFactory.Resolve();
                info.Initialise(this, entry);

                effects.Add(info.Entry.Id, info);
                log.LogTrace($"Added new SpellTargetEffectInfo for target {Guid} for spell {Collection.Spell.Spell4Id}, effect {entry.EffectType}.");
            }

            SpellEffectExecutionResult result = info.Execute(executionContext);
            if (result == SpellEffectExecutionResult.PreventEffect)
            {
                if (executionContext.IsDelayed)
                    info.Finish();
                else
                    effects.Remove(info.Entry.Id);
            }

            return result;
        }

        /// <summary>
        /// Finish all effects for the target.
        /// </summary>
        public void Finish()
        {
            if (IsFinalised)
                return;

            foreach (ISpellTargetEffectInfo targetEffectInfo in effects.Values)
                targetEffectInfo.Finish();

            log.LogTrace($"Finished all SpellTargetEffectInfo for target {Guid} spell {Collection.Spell.Spell4Id}.");
        }

        /// <summary>
        /// Return the <see cref="IUnitEntity"/> for the target.
        /// </summary>
        /// <remarks>
        /// This will attempt to find the <see cref="IUnitEntity"/> from visible units of the caster. 
        /// </remarks>
        public IUnitEntity GetTarget()
        {
            return Collection.Spell.Caster.GetVisible<IUnitEntity>(Guid);
        }

        private void SendBuffsRemoved(List<uint> unitIds)
        {
            if (unitIds.Count == 0)
                return;

            Collection.Spell.Caster.EnqueueToVisible(new ServerSpellBuffRemove
            {
                ServerUniqueId = Collection.Spell.CastingId,
                Targets        = unitIds
            }, true);
        }
    }
}
