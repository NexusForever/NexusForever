using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Abstract.Spell.Event;
using NexusForever.Game.Entity;
using NexusForever.Game.Prerequisite;
using NexusForever.Game.Spell.Event;
using NexusForever.Game.Static.Spell;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Model.Shared;
using NexusForever.Network.World.Message.Static;
using NexusForever.Script;
using NexusForever.Script.Template.Collection;
using NexusForever.Shared;
using NexusForever.Shared.Game;
using NLog;

namespace NexusForever.Game.Spell
{
    public partial class Spell : ISpell
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public ISpellParameters Parameters { get; }

        public IUnitEntity Caster => caster;
        protected readonly IUnitEntity caster;

        public uint CastingId { get; }
        public uint Spell4Id => Parameters.SpellInfo.Entry.Id;
        public CastMethod CastMethod { get; protected set; }

        public bool IsCasting => _IsCasting();
        public bool IsFinished => status == SpellStatus.Finished || status == SpellStatus.Failed;
        public bool IsFailed => status == SpellStatus.Failed;
        public bool IsWaiting => status == SpellStatus.Waiting;
        public bool HasGroup(uint groupId) => Parameters.SpellInfo.GroupList?.SpellGroupIds.Contains(groupId) ?? false;

        protected SpellStatus status
        {
            get => _status;
            set
            {
                if (_status == value)
                    return;

                var previousStatus = _status;
                _status = value;
                OnStatusChange(previousStatus, value);
            }
        }
        private SpellStatus _status;

        protected byte currentPhase = 255;
        protected uint duration = 0;

        protected readonly ISpellEventManager events = new SpellEventManager();

        protected readonly List<ISpellTargetInfo> targets = new();
        protected readonly List<ITelegraph> telegraphs = new();
        protected readonly List<IProxy> proxies = new();
        protected Dictionary<uint /*effectId*/, uint/*count*/> effectTriggerCount = new();
        protected Dictionary<uint /*effectId*/, double/*effectTimer*/> effectRetriggerTimers = new();

        private UpdateTimer persistCheck = new(0.1d);

        private IScriptCollection scriptCollection;

        public Spell(IUnitEntity caster, ISpellParameters parameters, CastMethod castMethod)
        {
            this.caster = caster;
            Parameters  = parameters;

            CastingId   = GlobalSpellManager.Instance.NextCastingId;
            CastMethod  = castMethod;
            status      = SpellStatus.Initiating;

            parameters.RootSpellInfo ??= parameters.SpellInfo;

            if (this is not SpellThreshold && parameters.SpellInfo.Thresholds.Count > 0)
                throw new NotImplementedException();

            scriptCollection = ScriptManager.Instance.InitialiseOwnedScripts<ISpell>(this, parameters.SpellInfo.Entry.Id);
        }

        public void Dispose()
        {
            if (scriptCollection != null)
                ScriptManager.Instance.Unload(scriptCollection);

            scriptCollection = null;
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public virtual void Update(double lastTick)
        {
            if (status == SpellStatus.Initiating)
                return;

            scriptCollection.Invoke<IUpdate>(s => s.Update(lastTick));

            events.Update(lastTick);
            CheckPersistance(lastTick);
        }

        /// <summary>
        /// Invoked each world tick, after Update() for this <see cref="ISpell"/>, with the delta since the previous tick occurred.
        /// </summary>
        public void LateUpdate(double lastTick)
        {
            if (CanFinish())
            {
                status = SpellStatus.Finished;

                if (Parameters.PositionalUnitId > 0)
                    caster.GetVisible<WorldEntity>(Parameters.PositionalUnitId)?.RemoveFromMap();

                foreach (ISpellTargetInfo target in targets)
                    RemoveEffects(target);

                SendSpellFinish();
                log.Trace($"Spell {Parameters.SpellInfo.Entry.Id} has finished.");
            }
        }

        /// <summary>
        /// Begin cast, checking prerequisites before initiating.
        /// </summary>
        protected bool CanCast()
        {
            if (status != SpellStatus.Initiating)
                throw new InvalidOperationException();

            log.Trace($"Spell {Parameters.SpellInfo.Entry.Id} has started initating.");

            CastResult result = CheckCast();
            if (result != CastResult.Ok)
            {
                // Swallow Proxy CastResults
                if (Parameters.IsProxy)
                    return false;

                if (caster is IPlayer)
                    (caster as IPlayer).SpellManager.SetAsContinuousCast(null);

                SendSpellCastResult(result);
                status = SpellStatus.Failed;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Begin cast, checking prerequisites before initiating.
        /// </summary>
        public virtual bool Cast()
        {
            if (!CanCast())
                return false;

            // TODO: Handle all GlobalCooldownEnums. It looks like it's just a "Type" that the GCD is stored against. Each spell checks the GCD for its type.
            if (caster is IPlayer player)
            {
                if (Parameters.SpellInfo.GlobalCooldown != null && !Parameters.IsProxy)
                    player.SpellManager.SetGlobalSpellCooldown(Parameters.SpellInfo.Entry.GlobalCooldownEnum, Parameters.SpellInfo.GlobalCooldown.CooldownTime / 1000d);
                else if (Parameters.IsProxy)
                    player.SpellManager.SetSpellCooldown(Parameters.SpellInfo, Parameters.CooldownOverride / 1000d);
            }

            // It's assumed that non-player entities will be stood still to cast (most do). 
            // TODO: There are a handful of telegraphs that are attached to moving units (specifically rotating units) which this needs to be updated to account for.
            if (!(caster is IPlayer))
                InitialiseTelegraphs();

            // TODO: Fire Script's OnCast

            return true;
        }

        /// <summary>
        /// Returns a <see cref="CastResult"/> describing whether or not this <see cref="ISpell"/> can be cast by its caster.
        /// </summary>
        protected CastResult CheckCast()
        {
            CastResult preReqCheck = CheckPrerequisites();
            if (preReqCheck != CastResult.Ok)
                return preReqCheck;

            CastResult ccResult = CheckCCConditions();
            if (ccResult != CastResult.Ok)
                return ccResult;

            if (caster is IPlayer player)
            {
                if (IsCasting && Parameters.UserInitiatedSpellCast && !Parameters.IsProxy)
                    return CastResult.SpellAlreadyCasting;

                // TODO: Some spells can be cast during other spell casts. Reflect that in this check
                if (caster.IsCasting() && Parameters.UserInitiatedSpellCast && !Parameters.IsProxy)
                    return CastResult.SpellAlreadyCasting;

                if (player.SpellManager.GetSpellCooldown(Parameters.SpellInfo.Entry.Id) > 0d &&
                    Parameters.UserInitiatedSpellCast &&
                    !Parameters.IsProxy)
                    return CastResult.SpellCooldown;

                foreach (SpellCoolDownEntry coolDownEntry in Parameters.SpellInfo.Cooldowns)
                {
                    if (player.SpellManager.GetSpellCooldownByCooldownId(coolDownEntry.Id) > 0d &&
                        Parameters.UserInitiatedSpellCast &&
                        !Parameters.IsProxy)
                        return CastResult.SpellCooldown;
                }

                if (player.SpellManager.GetGlobalSpellCooldown(Parameters.SpellInfo.Entry.GlobalCooldownEnum) > 0d &&
                    !Parameters.IsProxy &&
                    Parameters.UserInitiatedSpellCast)
                    return CastResult.SpellGlobalCooldown;

                if (Parameters.CharacterSpell?.MaxAbilityCharges > 0 && Parameters.CharacterSpell?.AbilityCharges == 0)
                    return CastResult.SpellNoCharges;

                CastResult resourceConditions = CheckResourceConditions();
                if (resourceConditions != CastResult.Ok)
                {
                    if (Parameters.UserInitiatedSpellCast && !Parameters.IsProxy)
                        player.SpellManager.SetAsContinuousCast(null);

                    return resourceConditions;
                }
            }

            return CastResult.Ok;
        }

        private CastResult CheckPrerequisites()
        {
            // TODO: Remove below line and evaluate PreReq's for Non-Player Entities
            if (caster is not IPlayer player)
                return CastResult.Ok;

            // Runners override the Caster Check, allowing the Caster to Cast the spell due to this Prerequisite being met
            if (Parameters.SpellInfo.CasterCastPrerequisite != null && !CheckRunnerOverride(player))
            {
                if (!PrerequisiteManager.Instance.Meets(player, Parameters.SpellInfo.CasterCastPrerequisite.Id))
                    return CastResult.PrereqCasterCast;
            }

            // not sure if this should be for explicit and/or implicit targets
            if (Parameters.SpellInfo.TargetCastPrerequisites != null)
            {
            }

            // this probably isn't the correct place, name implies this should be constantly checked
            if (Parameters.SpellInfo.CasterPersistencePrerequisites != null)
            {
            }

            if (Parameters.SpellInfo.TargetPersistencePrerequisites != null)
            {
            }

            return CastResult.Ok;
        }

        /// <summary>
        /// Returns whether the Caster is in a state where they can ignore Resource or other constraints.
        /// </summary>
        private bool CheckRunnerOverride(IPlayer player)
        {
            foreach (PrerequisiteEntry runnerPrereq in Parameters.SpellInfo.PrerequisiteRunners)
                if (PrerequisiteManager.Instance.Meets(player, runnerPrereq.Id))
                    return true;

            return false;
        }

        protected CastResult CheckCCConditions()
        {
            // TODO: this just looks like a mask for CCState enum
            if (Parameters.SpellInfo.CasterCCConditions != null)
            {
            }

            // not sure if this should be for explicit and/or implicit targets
            if (Parameters.SpellInfo.TargetCCConditions != null)
            {
            }

            return CastResult.Ok;
        }

        protected CastResult CheckResourceConditions()
        {
            if (!(caster is Player player))
                return CastResult.Ok;

            bool runnerOveride = CheckRunnerOverride(player);
            if (runnerOveride)
                return CastResult.Ok;

            //for (int i = 0; i < parameters.SpellInfo.Entry.CasterInnateRequirements.Length; i++)
            //{
            //    uint innateRequirement = parameters.SpellInfo.Entry.CasterInnateRequirements[i];
            //    if (innateRequirement == 0)
            //        continue;

            //    switch (parameters.SpellInfo.Entry.CasterInnateRequirementEval[i])
            //    {
            //        case 2:
            //            if (caster.GetVitalValue((Vital)innateRequirement) < parameters.SpellInfo.Entry.CasterInnateRequirementValues[i])
            //                return GlobalSpellManager.Instance.GetFailedCastResultForVital((Vital)innateRequirement);
            //            break;
            //    }
            //}

            //for (int i = 0; i < parameters.SpellInfo.Entry.InnateCostTypes.Length; i++)
            //{
            //    uint innateCostType = parameters.SpellInfo.Entry.InnateCostTypes[i];
            //    if (innateCostType == 0)
            //        continue;

            //    if (caster.GetVitalValue((Vital)innateCostType) < parameters.SpellInfo.Entry.InnateCosts[i])
            //        return GlobalSpellManager.Instance.GetFailedCastResultForVital((Vital)innateCostType);
            //}

            return CastResult.Ok;
        }

        /// <summary>
        /// Initialises a <see cref="ITelegraph"/> per <see cref="TelegraphDamageEntry"/> as associated with this <see cref="ISpell"/>.
        /// </summary>
        private void InitialiseTelegraphs()
        {
            telegraphs.Clear();

            Vector3 position = caster.Position;
            if (Parameters.PositionalUnitId > 0)
                position = caster.GetVisible<IWorldEntity>(Parameters.PositionalUnitId)?.Position ?? caster.Position;

            Vector3 rotation = caster.Rotation;
            if (Parameters.PositionalUnitId > 0)
                rotation = caster.GetVisible<IWorldEntity>(Parameters.PositionalUnitId)?.Rotation ?? caster.Rotation;

            foreach (TelegraphDamageEntry telegraphDamageEntry in Parameters.SpellInfo.Telegraphs)
                telegraphs.Add(new Telegraph(telegraphDamageEntry, caster, caster.Position, caster.Rotation));
        }

        /// <summary>
        /// Cancel cast with supplied <see cref="CastResult"/>.
        /// </summary>
        public virtual void CancelCast(CastResult result)
        {
            if (caster is IPlayer player && !player.IsLoading)
            {
                player.Session.EnqueueMessageEncrypted(new Server07F9
                {
                    ServerUniqueId = CastingId,
                    CastResult = result,
                    CancelCast = true
                });

                if (result == CastResult.CasterMovement)
                    player.SpellManager.SetGlobalSpellCooldown(Parameters.SpellInfo.Entry.GlobalCooldownEnum, 0d);

                player.SpellManager.SetAsContinuousCast(null);

                SendSpellCastResult(result);
            }

            events.CancelEvents();
            status = SpellStatus.Finishing;

            log.Trace($"Spell {Parameters.SpellInfo.Entry.Id} cast was cancelled.");
        }

        protected virtual void Execute(bool handleCDAndCost = true)
        {
            SpellStatus previousStatus = status;
            status = SpellStatus.Executing;
            log.Trace($"Spell {Parameters.SpellInfo.Entry.Id} has started executing.");

            if (handleCDAndCost)
            {
                if ((currentPhase == 0 || currentPhase == 255))
                {
                    CostSpell();
                    SetCooldown();
                }
            }

            // Clear Effects so that we don't duplicate Effect information back to the client.
            targets.ForEach(t => t.Effects.Clear());
            effectTriggerCount.Clear();

            // Order below must not change.
            SelectTargets();  // First Select Targets
            ExecuteEffects(); // All Effects are evaluated and executed (after SelectTargets())
            HandleProxies();  // Any Proxies that are added by Effets are evaluated and executed (after ExecuteEffects())
            SendSpellGo();    // Inform the Client once all evaluations are taken place (after Effects & Proxies are executed)

            if (duration > 0 || Parameters.SpellInfo.Entry.SpellDuration > 0)
                SendBuffsApplied(targets.Where(x => x.TargetSelectionState == TargetSelectionState.New).Select(x => x.Entity.Guid).ToList());
        }

        protected void HandleProxies()
        {
            foreach (IProxy proxy in proxies)
                proxy.Evaluate();

            foreach (IProxy proxy in proxies)
                proxy.Cast(caster, events);

            proxies.Clear();
        }

        protected void SetCooldown()
        {
            if (!(caster is IPlayer player))
                return;

            if (Parameters.SpellInfo.Entry.SpellCoolDown != 0u)
                player.SpellManager.SetSpellCooldown(Parameters.SpellInfo, Parameters.SpellInfo.Entry.SpellCoolDown / 1000d);
        }

        protected void CostSpell()
        {
            if (Parameters.CharacterSpell?.MaxAbilityCharges > 0)
                Parameters.CharacterSpell.UseCharge();

            // TODO: Handle costing Vital Resources
        }

        protected virtual void SelectTargets()
        {
            List<uint> uniqueTargets = new();

            // We clear targets every time this is called for this spell so we don't have duplicate targets.
            targets.Clear();

            // Add Caster Entity with the appropriate SpellEffectTargetFlags.
            targets.Add(new SpellTargetInfo(SpellEffectTargetFlags.Caster, caster));

            // Add Targeted Entity with the appropriate SpellEffectTargetFlags.
            if (Parameters.PrimaryTargetId > 0)
            {
                IUnitEntity primaryTargetEntity = caster.GetVisible<IUnitEntity>(Parameters.PrimaryTargetId);
                if (primaryTargetEntity != null)
                    targets.Add(new SpellTargetInfo((SpellEffectTargetFlags.Target), primaryTargetEntity));
            }
            else
                targets[0].Flags |= SpellEffectTargetFlags.Target;

            // Targeting First Pass: Do Basic Checks to get targets for spell as needed, nearby.
            targets.AddRange(new AoeSelection(caster, Parameters));

            // Re-initiailise Telegraphs at Execute time, so that position and rotation is calculated appropriately.
            // This is optimised to only happen on player-cast spells.
            // TODO: Add support for this for Server Controlled entities. It is presumed most will stand still when casting.
            if (caster is Player)
                InitialiseTelegraphs();

            if (telegraphs.Count > 0)
            {
                List<ISpellTargetInfo> allowedTargets = new();
                foreach (ITelegraph telegraph in telegraphs)
                {
                    List<uint> targetGuids = new();

                    // Ensure only telegraphs that apply to this Execute phase are evaluated.
                    if (CastMethod == CastMethod.Multiphase && currentPhase < 255)
                    {
                        int phaseMask = 1 << currentPhase;
                        if (telegraph.TelegraphDamage.PhaseFlags != 1 && (phaseMask & telegraph.TelegraphDamage.PhaseFlags) == 0)
                            continue;
                    }

                    log.Trace($"Getting targets for Telegraph ID {telegraph.TelegraphDamage.Id}");

                    foreach (var target in telegraph.GetTargets(this, targets))
                    {
                        // Ensure that this Telegraph hasn't already selected this entity
                        if (targetGuids.Contains(target.Entity.Guid))
                            continue;

                        // Ensure that this telegraph doesn't select an entity that has already been slected by another telegraph, as the targeting flags dictate.
                        if ((Parameters.SpellInfo.BaseInfo.Entry.TargetingFlags & 32) != 0 &&
                            uniqueTargets.Contains(target.Entity.Guid))
                            continue;

                        target.Flags |= SpellEffectTargetFlags.Telegraph;
                        allowedTargets.Add(target);
                        targetGuids.Add(target.Entity.Guid);
                        uniqueTargets.Add(target.Entity.Guid);
                    }

                    log.Trace($"Got {targets.Count} for Telegraph ID {telegraph.TelegraphDamage.Id}");
                }
                targets.RemoveAll(x => x.Flags == SpellEffectTargetFlags.Telegraph); // Only remove targets that are ONLY Telegraph Targeted
                targets.AddRange(allowedTargets);
            }

            if (Parameters.SpellInfo.AoeTargetConstraints != null)
            {
                List<ISpellTargetInfo> finalAoeTargets = new();
                foreach (var target in targets)
                {
                    // Ensure that we're not exceeding the amount of targets we can select
                    if (Parameters.SpellInfo.AoeTargetConstraints.TargetCount > 0 &&
                        finalAoeTargets.Count > Parameters.SpellInfo.AoeTargetConstraints.TargetCount)
                        break;

                    if ((target.Flags & SpellEffectTargetFlags.Telegraph) == 0)
                        continue;

                    finalAoeTargets.Add(target);
                }

                // Finalise targets for effect execution
                targets.RemoveAll(x => x.Flags == SpellEffectTargetFlags.Telegraph); // Only remove targets that are ONLY Telegraph Targeted
                targets.AddRange(finalAoeTargets);
            }

            var distinctList = targets.Distinct(new SpellTargetInfo.SpellTargetInfoComparer()).ToList();
            targets.Clear();
            targets.AddRange(distinctList);
        }

        private void ExecuteEffects()
        {
            if (targets.Where(t => t.TargetSelectionState == TargetSelectionState.New).Count() == 0)
                return;

            if (targets.Count > 0 && CastMethod == CastMethod.Aura)
                log.Trace($"New Targets found for {CastingId}, applying effects.");

            // Using For..Loop instead of foreach intentionally, as this can be modified as effects are evaluated.
            for (int index = 0; index < Parameters.SpellInfo.Effects.Count(); index++)
            {
                Spell4EffectsEntry spell4EffectsEntry = Parameters.SpellInfo.Effects[index];

                ExecuteEffect(spell4EffectsEntry);
            }
        }

        private bool CanExecuteEffect(Spell4EffectsEntry spell4EffectsEntry)
        {
            if (caster is IPlayer player)
            {
                // Ensure caster can apply this effect
                if (spell4EffectsEntry.PrerequisiteIdCasterApply > 0 && !PrerequisiteManager.Instance.Meets(player, spell4EffectsEntry.PrerequisiteIdCasterApply))
                    return false;
            }

            if (CastMethod == CastMethod.Multiphase && currentPhase < 255)
            {
                int phaseMask = 1 << currentPhase;
                if ((spell4EffectsEntry.PhaseFlags != 1 && spell4EffectsEntry.PhaseFlags != uint.MaxValue) && (phaseMask & spell4EffectsEntry.PhaseFlags) == 0)
                    return false;
            }

            if (CastMethod == CastMethod.Aura && spell4EffectsEntry.TickTime > 0 && effectRetriggerTimers[spell4EffectsEntry.Id] > 0d)
                return false;

            return true;
        }

        protected void ExecuteEffect(Spell4EffectsEntry spell4EffectsEntry)
        {
            if (!CanExecuteEffect(spell4EffectsEntry))
                return;

            log.Trace($"Executing SpellEffect ID {spell4EffectsEntry.Id} ({1 << currentPhase})");

            // Set Allowed States for entities being affected by this ExecuteEffect
            List<TargetSelectionState> allowedStates = new() { TargetSelectionState.New };
            if (CastMethod == CastMethod.Aura && spell4EffectsEntry.TickTime > 0)
                allowedStates.Add(TargetSelectionState.Existing);

            // select targets for effect
            List<ISpellTargetInfo> effectTargets = targets
                .Where(t => allowedStates.Contains(t.TargetSelectionState) && (t.Flags & (SpellEffectTargetFlags)spell4EffectsEntry.TargetFlags) != 0)
                .ToList();

            SpellEffectDelegate handler = GlobalSpellManager.Instance.GetEffectHandler((SpellEffectType)spell4EffectsEntry.EffectType);
            if (handler == null)
                log.Warn($"Unhandled spell effect {(SpellEffectType)spell4EffectsEntry.EffectType}");
            else
            {
                uint effectId = GlobalSpellManager.Instance.NextEffectId;
                foreach (ISpellTargetInfo effectTarget in effectTargets)
                {
                    if (!CheckEffectApplyPrerequisites(spell4EffectsEntry, effectTarget.Entity, effectTarget.Flags))
                        continue;

                    var info = new SpellTargetInfo.SpellTargetEffectInfo(effectId, spell4EffectsEntry);
                    effectTarget.Effects.Add(info);

                    // TODO: if there is an unhandled exception in the handler, there will be an infinite loop on Execute()
                    handler.Invoke(this, effectTarget.Entity, info);

                    // Track the number of times this effect has fired.
                    // Some spell effects have a limited trigger count per spell cast.
                    if (effectTriggerCount.TryGetValue(spell4EffectsEntry.Id, out uint count))
                        effectTriggerCount[spell4EffectsEntry.Id]++;
                    else
                        effectTriggerCount.TryAdd(spell4EffectsEntry.Id, 1);
                }

                // Add durations for each effect so that when the Effect timer runs out, the Spell can Finish.
                if (spell4EffectsEntry.DurationTime > 0)
                    events.EnqueueEvent(new SpellEvent(spell4EffectsEntry.DurationTime / 1000d, () => { /* placeholder for duration */ }));

                if (spell4EffectsEntry.DurationTime > 0 && spell4EffectsEntry.DurationTime > duration)
                    duration = spell4EffectsEntry.DurationTime;

                if (spell4EffectsEntry.DurationTime == 0u && ((SpellEffectFlags)spell4EffectsEntry.Flags & SpellEffectFlags.CancelOnly) != 0)
                    Parameters.ForceCancelOnly = true;

                if (spell4EffectsEntry.TickTime > 0 && effectRetriggerTimers.ContainsKey(spell4EffectsEntry.Id))
                    effectRetriggerTimers[spell4EffectsEntry.Id] = spell4EffectsEntry.TickTime / 1000d;
            }
        }

        protected void RemoveEffects(ISpellTargetInfo target)
        {
            if (target.Entity == null)
                return;

            if (targets.Count > 0 && CastMethod == CastMethod.Aura)
                log.Trace($"Target exited spell {CastingId}'s range, removing effects.");

            // TODO: Remove effects triggered by this spell from the target.
            // target.Entity?.RemoveSpellProperties(Spell4Id);
            // target.Entity?.RemoveProc(parameters.SpellInfo.Entry.Id);
            // target.Entity?.RemoveTemporaryDisplayItem(Spell4Id);
        }

        private bool CheckEffectApplyPrerequisites(Spell4EffectsEntry spell4EffectsEntry, IUnitEntity unit, SpellEffectTargetFlags targetFlags)
        {
            bool effectCanApply = true;

            // TODO: Possibly update Prereq Manager to handle other Units
            if (unit is not IPlayer player)
                return true;

            if ((targetFlags & SpellEffectTargetFlags.Caster) != 0)
            {
                // TODO
                if (spell4EffectsEntry.PrerequisiteIdCasterApply > 0)
                {
                    effectCanApply = PrerequisiteManager.Instance.Meets(player, spell4EffectsEntry.PrerequisiteIdCasterApply);
                }
            }

            if (effectCanApply && (targetFlags & SpellEffectTargetFlags.Caster) == 0)
            {
                if (spell4EffectsEntry.PrerequisiteIdTargetApply > 0)
                {
                    effectCanApply = PrerequisiteManager.Instance.Meets(player, spell4EffectsEntry.PrerequisiteIdTargetApply);
                }
            }

            return effectCanApply;
        }

        public bool IsMovingInterrupted()
        {
            // TODO: implement correctly
            return Parameters.UserInitiatedSpellCast && Parameters.SpellInfo.BaseInfo.SpellType.Id != 5 && Parameters.SpellInfo.Entry.CastTime > 0;
        }

        /// <summary>
        /// Finish this <see cref="ISpell"/> and end all effects associated with it.
        /// </summary>
        public virtual void Finish()
        {
            if (status == SpellStatus.Finished)
                return;

            events.CancelEvents();
            status = SpellStatus.Finishing;
        }

        private bool PassEntityChecks()
        {
            if (caster is IPlayer)
                return Parameters.UserInitiatedSpellCast;

            return true;
        }

        protected virtual bool _IsCasting()
        {
            if (Parameters.IsProxy)
                return false;

            if ((caster is not IPlayer) && status == SpellStatus.Initiating)
                return true;

            return PassEntityChecks();
        }

        protected void SendSpellCastResult(CastResult castResult)
        {
            if (castResult == CastResult.Ok)
                return;

            log.Trace($"Spell {Parameters.SpellInfo.Entry.Id} failed to cast {castResult}.");

            if (caster is IPlayer player && !player.IsLoading)
            {
                player.Session.EnqueueMessageEncrypted(new ServerSpellCastResult
                {
                    Spell4Id   = Parameters.SpellInfo.Entry.Id,
                    CastResult = castResult
                });
            }
        }

        protected virtual uint GetPrimaryTargetId()
        {
            if (Parameters.PrimaryTargetId > 0)
                return Parameters.PrimaryTargetId;

            if (Parameters.PositionalUnitId > 0)
                return Parameters.PositionalUnitId;

            return caster.Guid;
        }

        protected void SendSpellStart()
        {
            var spellStart = new ServerSpellStart
            {
                CastingId              = CastingId,
                CasterId               = caster.Guid,
                PrimaryTargetId        = GetPrimaryTargetId(),
                Spell4Id               = Parameters.SpellInfo.Entry.Id,
                RootSpell4Id           = Parameters.RootSpellInfo?.Entry.Id ?? 0,
                ParentSpell4Id         = Parameters.ParentSpellInfo?.Entry.Id ?? 0,
                FieldPosition          = new Position(caster.Position),
                Yaw                    = caster.Rotation.X,
                UserInitiatedSpellCast = Parameters.UserInitiatedSpellCast,
                InitialPositionData    = new List<InitialPosition>(),
                TelegraphPositionData  = new List<TelegraphPosition>()
            };

            // TODO: Add Proxy Units
            List<IUnitEntity> unitsCasting = new List<IUnitEntity>();
            unitsCasting.Add(caster);

            foreach (IUnitEntity unit in unitsCasting)
            {
                if (unit == null)
                    continue;

                if (unit is IPlayer)
                    continue;

                spellStart.InitialPositionData.Add(new InitialPosition
                {
                    UnitId      = unit.Guid,
                    Position    = new Position(unit.Position),
                    TargetFlags = 3,
                    Yaw         = unit.Rotation.X
                });
            }

            foreach (IUnitEntity unit in unitsCasting)
            {
                if (unit == null)
                    continue;

                foreach (ITelegraph telegraph in telegraphs)
                    spellStart.TelegraphPositionData.Add(new TelegraphPosition
                    {
                        TelegraphId    = (ushort)telegraph.TelegraphDamage.Id,
                        AttachedUnitId = unit.Guid,
                        TargetFlags    = 3,
                        Position       = new Position(telegraph.Position),
                        Yaw            = telegraph.Rotation.X
                    });
            }

            caster.EnqueueToVisible(spellStart, true);
        }

        private void SendSpellFinish()
        {
            if (status != SpellStatus.Finished)
                return;

            caster.EnqueueToVisible(new ServerSpellFinish
            {
                ServerUniqueId = CastingId,
            }, true);
        }

        private void SendSpellGo()
        {
            if (CastMethod == CastMethod.Aura && targets.FirstOrDefault(x => x.TargetSelectionState == TargetSelectionState.New) == null)
                return;

            var serverSpellGo = new ServerSpellGo
            {
                ServerUniqueId     = CastingId,
                PrimaryDestination = new Position(caster.Position),
                Phase              = currentPhase
            };

            byte targetCount = 0;
            foreach (ISpellTargetInfo targetInfo in targets
                .Where(t => t.Effects.Count > 0 && t.TargetSelectionState == TargetSelectionState.New))
            {
                var networkTargetInfo = new TargetInfo
                {
                    UnitId        = targetInfo.Entity.Guid,
                    Ndx           = targetCount++,
                    TargetFlags   = (byte)targetInfo.Flags,
                    InstanceCount = 1,
                    CombatResult  = CombatResult.Hit
                };

                foreach (ISpellTargetEffectInfo targetEffectInfo in targetInfo.Effects)
                {

                    if ((SpellEffectType)targetEffectInfo.Entry.EffectType == SpellEffectType.Proxy)
                        continue;

                    var networkTargetEffectInfo = new EffectInfo
                    {
                        Spell4EffectId = targetEffectInfo.Entry.Id,
                        EffectUniqueId = targetEffectInfo.EffectId,
                        DelayTime = targetEffectInfo.Entry.DelayTime,
                        TimeRemaining = duration > 0 ? (int)duration : -1
                    };

                    if (targetEffectInfo.Damage != null)
                    {
                        networkTargetEffectInfo.InfoType = 1;
                        networkTargetEffectInfo.DamageDescriptionData = new DamageDescription
                        {
                            RawDamage = targetEffectInfo.Damage.RawDamage,
                            RawScaledDamage = targetEffectInfo.Damage.RawScaledDamage,
                            AbsorbedAmount = targetEffectInfo.Damage.AbsorbedAmount,
                            ShieldAbsorbAmount = targetEffectInfo.Damage.ShieldAbsorbAmount,
                            AdjustedDamage = targetEffectInfo.Damage.AdjustedDamage,
                            OverkillAmount = targetEffectInfo.Damage.OverkillAmount,
                            KilledTarget = targetEffectInfo.Damage.KilledTarget,
                            CombatResult = CombatResult.Hit,
                            DamageType = targetEffectInfo.Damage.DamageType
                        };
                    }

                    networkTargetInfo.EffectInfoData.Add(networkTargetEffectInfo);
                }

                serverSpellGo.TargetInfoData.Add(networkTargetInfo);
            }

            var unitsCasting = new List<IUnitEntity>
            {
                caster
            };

            foreach (IUnitEntity unit in unitsCasting)
                serverSpellGo.InitialPositionData.Add(new InitialPosition
                {
                    UnitId      = unit.Guid,
                    Position    = new Position(unit.Position),
                    TargetFlags = 3,
                    Yaw         = unit.Rotation.X
                });

            foreach (IUnitEntity unit in unitsCasting)
                foreach (ITelegraph telegraph in telegraphs)
                    serverSpellGo.TelegraphPositionData.Add(new TelegraphPosition
                    {
                        TelegraphId    = (ushort)telegraph.TelegraphDamage.Id,
                        AttachedUnitId = unit.Guid,
                        TargetFlags    = 3,
                        Position       = new Position(telegraph.Position),
                        Yaw            = telegraph.Rotation.X
                    });

            caster.EnqueueToVisible(serverSpellGo, true);
        }

        private void SendBuffsApplied(List<uint> unitIds)
        {
            if (unitIds.Count == 0)
                return;

            var serverSpellBuffsApply = new ServerSpellBuffsApply();
            foreach (uint unitId in unitIds)
                serverSpellBuffsApply.spellTargets.Add(new ServerSpellBuffsApply.SpellTarget
                {
                    ServerUniqueId = CastingId,
                    TargetId       = unitId,
                    InstanceCount  = 1 // TODO: If something stacks, we may need to grab this from the target unit
                });
            caster.EnqueueToVisible(serverSpellBuffsApply, true);
        }

        public void SendBuffsRemoved(List<uint> unitIds)
        {
            if (unitIds.Count == 0)
                return;

            ServerSpellBuffsRemoved serverSpellBuffsRemoved = new ServerSpellBuffsRemoved
            {
                CastingId = CastingId,
                SpellTargets = unitIds
            };
            caster.EnqueueToVisible(serverSpellBuffsRemoved, true);
        }

        private void SendRemoveBuff(uint unitId)
        {
            if (!Parameters.SpellInfo.BaseInfo.HasIcon)
                throw new InvalidOperationException();

            caster.EnqueueToVisible(new ServerSpellBuffRemove
            {
                CastingId = CastingId,
                CasterId  = unitId
            }, true);
        }

        private void CheckPersistance(double lastTick)
        {
            if (caster is not IPlayer player)
                return;

            if (Parameters.SpellInfo.Entry.PrerequisiteIdCasterPersistence == 0 && Parameters.SpellInfo.Entry.PrerequisiteIdTargetPersistence == 0)
                return;

            persistCheck.Update(lastTick);
            if (persistCheck.HasElapsed)
            {
                if (Parameters.SpellInfo.Entry.PrerequisiteIdCasterPersistence > 0 && !PrerequisiteManager.Instance.Meets(player, Parameters.SpellInfo.Entry.PrerequisiteIdCasterPersistence))
                    Finish();

                // TODO: Check if target can still persist

                persistCheck.Reset();
            }
        }

        /// <summary>
        /// Called when this <see cref="ISpell"/> <see cref="SpellStatus"/> changes.
        /// </summary>
        protected virtual void OnStatusChange(SpellStatus previousStatus, SpellStatus status)
        {
            if (status == SpellStatus.Casting)
                SendSpellStart();
        }

        protected virtual bool CanFinish()
        {
            return (status == SpellStatus.Executing && !events.HasPendingEvent && !Parameters.ForceCancelOnly) || status == SpellStatus.Finishing;
        }

        /// <summary>
        /// Add a <see cref="IProxy"/> to this spell's execution queue.
        /// </summary>
        /// <param name="proxy">Proxy instance to add</param>
        public void AddProxy(IProxy proxy)
        {
            proxies.Add(proxy);
        }

        /// <summary>
        /// Returns number of times a certain effect has been triggered, for this spell cast, with a given ID.
        /// </summary>
        /// <param name="effectId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public bool GetEffectTriggerCount(uint effectId, out uint count)
        {
            return effectTriggerCount.TryGetValue(effectId, out count);
        }
    }
}
