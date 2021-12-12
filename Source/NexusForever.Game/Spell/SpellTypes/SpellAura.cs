using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Entity;
using NexusForever.Game.Spell.Event;
using NexusForever.Game.Static.Spell;
using NexusForever.GameTable.Model;
using NexusForever.Shared.Game;
using NLog;

namespace NexusForever.Game.Spell
{
    [SpellType(CastMethod.Aura)]
    public partial class SpellAura : Spell, ISpellType
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private static CastMethod castMethod = CastMethod.Aura;

        private UpdateTimer auraExecute = new(0.1d);

        public SpellAura(IUnitEntity caster, ISpellParameters parameters) 
            : base(caster, parameters, castMethod)
        {
        }

        public override void Update(double lastTick)
        {
            base.Update(lastTick);

            if (status == SpellStatus.Executing && CastMethod == CastMethod.Aura)
            {
                auraExecute.Update(lastTick);
                if (auraExecute.HasElapsed)
                {
                    Execute();

                    var removeList = targets.Where(t => t.TargetSelectionState == TargetSelectionState.Old).ToList();
                    foreach (SpellTargetInfo target in removeList)
                    {
                        RemoveEffects(target);
                        targets.Remove(target);
                    }
                    if (removeList.Count > 0)
                        SendBuffsRemoved(removeList.Where(i => i.Entity != null).Select(i => i.Entity.Guid).ToList());

                    auraExecute.Reset();
                }

                foreach ((uint effectId, double timer) in effectRetriggerTimers)
                {
                    effectRetriggerTimers[effectId] -= lastTick;

                    if (timer <= 0d)
                    {
                        Spell4EffectsEntry spell4EffectsEntry = Parameters.SpellInfo.Effects.First(i => i.Id == effectId);
                        ExecuteEffect(spell4EffectsEntry);
                        HandleProxies();
                    }
                }
            }
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
            {
                if ((SpellEffectType)effect.EffectType != SpellEffectType.Proxy)
                {
                    log.Warn($"Aura (Spell4 {Spell4Id}) has unhandled effect type {(SpellEffectType)effect.EffectType}.");
                    continue;
                }

                effectRetriggerTimers.Add(effect.Id, effect.TickTime / 1000d);
            }

            if (Parameters.SpellInfo.Entry.SpellDuration > 0 && Parameters.SpellInfo.Entry.SpellDuration < uint.MaxValue)
                events.EnqueueEvent(new SpellEvent(Parameters.SpellInfo.Entry.SpellDuration / 1000d, Finish));

            if (Parameters.SpellInfo.BaseInfo.Entry.Creature2IdPositionalAoe > 0)
            {
                Simple positionalEntity = new Simple(Parameters.SpellInfo.BaseInfo.Entry.Creature2IdPositionalAoe, (entity) =>
                {
                    entity.Rotation = caster.Rotation;
                    Parameters.PositionalUnitId = entity.Guid;

                    status = SpellStatus.Casting;
                    log.Trace($"Spell {Parameters.SpellInfo.Entry.Id} has started casting.");
                });

                caster.Map.EnqueueAdd(positionalEntity, new Map.MapPosition
                {
                    Info = new Map.MapInfo
                    {
                        Entry = caster.Map.Entry
                    },
                    Position = caster.Position
                });
            }
            else
            {
                status = SpellStatus.Casting;
                log.Trace($"Spell {Parameters.SpellInfo.Entry.Id} has started casting.");
            }
            return true;
        }

        protected override bool _IsCasting()
        {
            return base._IsCasting() && status == SpellStatus.Casting;
        }

        protected override void SelectTargets()
        {
            List<ISpellTargetInfo> previousTargets = targets.ToList();

            base.SelectTargets();

            // Use the previous targets list to mark target selection appropriately.
            foreach (ISpellTargetInfo spellTarget in previousTargets)
            {
                // Entity exists, again, in this selection
                var existingTarget = targets.FirstOrDefault(t => t.Entity.Guid == spellTarget.Entity.Guid && t.Flags == spellTarget.Flags);
                if (existingTarget != null)
                {
                    existingTarget.TargetSelectionState = TargetSelectionState.Existing;
                    continue;
                }

                // If we've re-added the caster, just ignore this check. Caster SpellTarget exists every Target Selection, for tracking of Caster-only effects.
                if (spellTarget.Flags.HasFlag(SpellEffectTargetFlags.Caster) &&
                    !spellTarget.Flags.HasFlag(SpellEffectTargetFlags.Telegraph) &&
                    targets.FirstOrDefault(
                        t => t.Entity.Guid == spellTarget.Entity.Guid &&
                        t.TargetSelectionState == TargetSelectionState.New &&
                        t.Flags.HasFlag(SpellEffectTargetFlags.Telegraph)) != null
                    )
                    continue;

                // If caster is not in telegraph range, set them to an Existing selection state
                if (spellTarget.Flags.HasFlag(SpellEffectTargetFlags.Caster) &&
                    !spellTarget.Flags.HasFlag(SpellEffectTargetFlags.Caster) &&
                    targets.FirstOrDefault(
                        t => t.Entity.Guid == spellTarget.Entity.Guid &&
                        t.TargetSelectionState == TargetSelectionState.New &&
                        !t.Flags.HasFlag(SpellEffectTargetFlags.Telegraph)) != null)
                {
                    targets.FirstOrDefault(x => x.Entity.Guid == spellTarget.Entity.Guid).TargetSelectionState = TargetSelectionState.Existing;
                    continue;
                }

                // Entity is no longer a target, add to list and mark as Old.
                spellTarget.TargetSelectionState = TargetSelectionState.Old;
                targets.Add(spellTarget);
            }

            // Re-order based on selection state. Existing > New > Old.
            // This means that entities who already had effects will continue to receive it.
            targets.OrderBy(x => x.TargetSelectionState);

            // Re-check targets to ensure we've not collected more targets as "hittable" than we are allowed.
            var tempTargets = targets.ToList();
            for (int i = 0; i < targets.Count; i++)
            {
                var target = tempTargets[i];

                // Casters always remain targetable
                if (target.Flags == SpellEffectTargetFlags.Caster)
                    continue;

                // Nothing needs to be adjusted to entities marked as Old.
                if (target.TargetSelectionState == TargetSelectionState.Old)
                    continue;

                if (Parameters.SpellInfo.AoeTargetConstraints.TargetCount > 0 &&
                        i > Parameters.SpellInfo.AoeTargetConstraints.TargetCount)
                {
                    if (target.TargetSelectionState == TargetSelectionState.New)
                        targets.Remove(target); // Remove target if they are a new target, nothing needs to happen to them further this execution
                    else if (target.TargetSelectionState == TargetSelectionState.Existing)
                        targets[i].TargetSelectionState = TargetSelectionState.Old; // Mark an existing target as old, they will not receive effects this execution
                }
            }
        }
    }
}
