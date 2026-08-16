using System.Numerics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Abstract.Spell.Event;
using NexusForever.Game.Abstract.Spell.Target;
using NexusForever.Game.Abstract.Spell.Target.Implicit;
using NexusForever.Game.Abstract.Spell.Target.Implicit.Filter;
using NexusForever.Game.Abstract.Spell.Validator;
using NexusForever.Game.Prerequisite;
using NexusForever.Game.Spell.Event;
using NexusForever.Game.Spell.Target;
using NexusForever.Game.Spell.Type;
using NexusForever.Game.Static;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Spell;
using NexusForever.Game.Static.Spell.Effect;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Combat;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Model.Shared;
using NexusForever.Network.World.Message.Static;
using NexusForever.Script;
using NexusForever.Script.Template.Collection;
using NexusForever.Shared;
using NexusForever.Shared.Game;

namespace NexusForever.Game.Spell
{
    public abstract class Spell : ISpell
    {
        public abstract CastMethod CastMethod { get; }

        public ISpellParameters Parameters { get; private set; }
        public IUnitEntity Caster { get; private set; }
        public uint CastingId { get; private set; }
        public uint Spell4Id => Parameters.SpellInfo.Entry.Id;

        public bool IsCasting => _IsCasting();
        public bool IsFinished => status == SpellStatus.Finished || status == SpellStatus.Failed;
        public bool IsFailed => status == SpellStatus.Failed;
        public bool IsWaiting => status == SpellStatus.Waiting;

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

        protected readonly List<ITelegraph> telegraphs = [];

        private readonly UpdateTimer persistCheck = new(TimeSpan.FromMilliseconds(100));

        private readonly Dictionary<Spell4EffectsEntry, UpdateTimer> delayedEffects = [];

        private IScriptCollection scriptCollection;

        #region Dependency Injection

        private readonly ILogger log;
        private readonly ISpellTargetInfoCollection spellTargetInfoCollection;
        private readonly IGlobalSpellManager globalSpellManager;
        private readonly ICastResultValidatorManager castResultValidatorManager;
        private readonly IDisableManager disableManager;

        public Spell(
            ILogger log,
            ISpellTargetInfoCollection spellTargetInfoCollection,
            IGlobalSpellManager globalSpellManager,
            ICastResultValidatorManager castResultValidatorManager,
            IDisableManager disableManager)
        {
            this.log                        = log;
            this.spellTargetInfoCollection  = spellTargetInfoCollection;
            this.globalSpellManager         = globalSpellManager;
            this.castResultValidatorManager = castResultValidatorManager;
            this.disableManager             = disableManager;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="ISpell"/> with the supplied <see cref="IUnitEntity"/> and <see cref="ISpellParameters"/>.
        /// </summary>
        public virtual void Initialise(IUnitEntity caster, ISpellParameters parameters)
        {
            if (Caster != null)
                throw new InvalidOperationException();

            Caster     = caster;
            Parameters = parameters;
            CastingId  = globalSpellManager.NextCastingId;

            status = SpellStatus.Initiating;

            parameters.RootSpellInfo ??= parameters.SpellInfo;

            if (this is not SpellThreshold && parameters.SpellInfo.Thresholds.Count > 0)
                throw new NotImplementedException();

            spellTargetInfoCollection.Initialise(this);

            scriptCollection = ScriptManager.Instance.InitialiseOwnedCollection<ISpell>(this);
            ScriptManager.Instance.InitialiseOwnedScripts<ISpell>(scriptCollection, parameters.SpellInfo.Entry.Id);
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

            spellTargetInfoCollection.Update(lastTick);
            HandleDelayedEffects(lastTick);

            scriptCollection.Invoke<IUpdate>(s => s.Update(lastTick));

            events.Update(lastTick);
            CheckPersistance(lastTick);
        }

        private void HandleDelayedEffects(double lastTick)
        {
            var executionContext = new SpellExecutionContext();
            executionContext.Initialise(this, true);

            foreach ((Spell4EffectsEntry effect, UpdateTimer updateTimer) in delayedEffects)
            {
                updateTimer.Update(lastTick);
                if (updateTimer.HasElapsed)
                {
                    executionContext.AddSpellEffect(effect);
                    delayedEffects.Remove(effect);
                }
            }

            Execute(executionContext);
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
                    Caster.SummonFactory.Unsummon(Parameters.PositionalUnitId);

                SendSpellFinish();
                log.LogTrace($"Spell {Parameters.SpellInfo.Entry.Id} has finished.");
            }
        }

        /// <summary>
        /// Return <see cref="ISpellTargetInfo"/> for the supplied <see cref="IUnitEntity"/>.
        /// </summary>
        public ISpellTargetInfo GetTarget(IUnitEntity entity)
        {
            return spellTargetInfoCollection.GetSpellTargetInfo(new SpellTarget(entity, SpellEffectTargetFlags.None));
        }

        /// <summary>
        /// Begin cast, checking prerequisites before initiating.
        /// </summary>
        protected bool CanCast()
        {
            if (status != SpellStatus.Initiating)
                throw new InvalidOperationException();

            log.LogTrace($"Spell {Parameters.SpellInfo.Entry.Id} has started initating.");

            CastResult result = CheckCast();
            if (result != CastResult.Ok)
            {
                // Swallow Proxy CastResults
                if (Parameters.IsProxy)
                    return false;

                if (Caster is IPlayer player)
                    player.SpellManager.SetAsContinuousCast(null);

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
            if (Caster is IPlayer player)
            {
                if (Parameters.SpellInfo.GlobalCooldown != null && !Parameters.IsProxy)
                    player.SpellManager.SetGlobalSpellCooldown(Parameters.SpellInfo.Entry.GlobalCooldownEnum, Parameters.SpellInfo.GlobalCooldown.CooldownTime / 1000d);
                else if (Parameters.IsProxy)
                    player.SpellManager.SetSpellCooldown(Parameters.SpellInfo, Parameters.CooldownOverride / 1000d);
            }

            // It's assumed that non-player entities will be stood still to cast (most do). 
            // TODO: There are a handful of telegraphs that are attached to moving units (specifically rotating units) which this needs to be updated to account for.
            if (Caster is not IPlayer)
                InitialiseTelegraphs();

            // TODO: Fire Script's OnCast

            return true;
        }

        /// <summary>
        /// Returns a <see cref="CastResult"/> describing whether or not this <see cref="ISpell"/> can be cast by its caster.
        /// </summary>
        protected CastResult CheckCast()
        {
            // TODO: replace the rest of this method with cast result validators...
            CastResult castResult = castResultValidatorManager.GetCastResult(this);
            if (castResult != CastResult.Ok)
                return castResult;

            CastResult preReqCheck = CheckPrerequisites();
            if (preReqCheck != CastResult.Ok)
                return preReqCheck;

            if (Caster is IPlayer player)
            {
                if (IsCasting && Parameters.UserInitiatedSpellCast && !Parameters.IsProxy)
                    return CastResult.SpellAlreadyCasting;

                // TODO: Some spells can be cast during other spell casts. Reflect that in this check
                if (Caster.IsCasting() && Parameters.UserInitiatedSpellCast && !Parameters.IsProxy)
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
            IUnitEntity target = GetExplicitTarget();

            // Runners override the Caster Check, allowing the Caster to Cast the spell due to this Prerequisite being met
            if (Parameters.SpellInfo.CasterCastPrerequisite != null && !CheckRunnerOverride(Caster))
            {
                if (!PrerequisiteManager.Instance.Meets(Caster, Parameters.SpellInfo.CasterCastPrerequisite.Id, target))
                    return CastResult.PrereqCasterCast;
            }

            if (Parameters.SpellInfo.TargetCastPrerequisite != null)
            {
                if (target == null)
                    return CastResult.PrereqTargetCast;

                if (!PrerequisiteManager.Instance.Meets(target, Parameters.SpellInfo.TargetCastPrerequisite.Id, Caster))
                    return CastResult.PrereqTargetCast;
            }

            return CastResult.Ok;
        }

        /// <summary>
        /// Returns whether the Caster is in a state where they can ignore Resource or other constraints.
        /// </summary>
        private bool CheckRunnerOverride(IUnitEntity subject)
        {
            foreach (PrerequisiteEntry runnerPrereq in Parameters.SpellInfo.PrerequisiteRunners)
                if (PrerequisiteManager.Instance.Meets(subject, runnerPrereq.Id))
                    return true;

            return false;
        }

        protected CastResult CheckResourceConditions()
        {
            if (Caster is not IPlayer player)
                return CastResult.Ok;

            bool runnerOveride = CheckRunnerOverride(player);
            if (runnerOveride)
                return CastResult.Ok;

            for (int i = 0; i < Parameters.SpellInfo.Entry.CasterInnateRequirements.Length; i++)
            {
                uint innateRequirement = Parameters.SpellInfo.Entry.CasterInnateRequirements[i];
                if (innateRequirement == 0)
                    continue;

                switch (Parameters.SpellInfo.Entry.CasterInnateRequirementEval[i])
                {
                    case 2:
                        if (Caster.GetVitalValue((Vital)innateRequirement) < Parameters.SpellInfo.Entry.CasterInnateRequirementValues[i])
                            return GlobalSpellManager.Instance.GetFailedCastResultForVital((Vital)innateRequirement);
                        break;
                }
            }

            for (int i = 0; i < Parameters.SpellInfo.Entry.InnateCostTypes.Length; i++)
            {
                uint innateCostType = Parameters.SpellInfo.Entry.InnateCostTypes[i];
                if (innateCostType == 0)
                    continue;

                if (Caster.GetVitalValue((Vital)innateCostType) < Parameters.SpellInfo.Entry.InnateCosts[i])
                    return GlobalSpellManager.Instance.GetFailedCastResultForVital((Vital)innateCostType);
            }

            return CastResult.Ok;
        }

        /// <summary>
        /// Initialises a <see cref="ITelegraph"/> per <see cref="TelegraphDamageEntry"/> as associated with this <see cref="ISpell"/>.
        /// </summary>
        private void InitialiseTelegraphs()
        {
            telegraphs.Clear();

            Vector3 position = Caster.Position;
            if (Parameters.PositionalUnitId > 0)
                position = Caster.GetVisible<IWorldEntity>(Parameters.PositionalUnitId)?.Position ?? Caster.Position;

            Vector3 rotation = Caster.Rotation;
            if (Parameters.PositionalUnitId > 0)
                rotation = Caster.GetVisible<IWorldEntity>(Parameters.PositionalUnitId)?.Rotation ?? Caster.Rotation;

            foreach (TelegraphDamageEntry telegraphDamageEntry in Parameters.SpellInfo.Telegraphs)
                if (IsTelegraphValid(telegraphDamageEntry))
                    telegraphs.Add(new Telegraph(telegraphDamageEntry, Caster, Caster.Position, Caster.Rotation));
        }

        protected virtual bool IsTelegraphValid(TelegraphDamageEntry telegraph)
        {
            // by default, all telegraphs are valid
            return true;
        }

        /// <summary>
        /// Cancel cast with supplied <see cref="CastResult"/>.
        /// </summary>
        public virtual void CancelCast(CastResult result)
        {
            if (!IsCasting)
                return;

            if (Caster is IPlayer player && !player.IsLoading)
            {
                player.Session.EnqueueMessageEncrypted(new Server07F9
                {
                    ServerUniqueId = CastingId,
                    CastResult     = result,
                    CancelCast     = true
                });

                if (result == CastResult.CasterMovement)
                    player.SpellManager.SetGlobalSpellCooldown(Parameters.SpellInfo.Entry.GlobalCooldownEnum, 0d);

                player.SpellManager.SetAsContinuousCast(null);

                SendSpellCastResult(result);
            }

            Finish();

            log.LogTrace($"Spell {Parameters.SpellInfo.Entry.Id} cast was cancelled.");
        }

        protected virtual void Execute(bool handleCDAndCost = true)
        {
            if (handleCDAndCost)
            {
                if ((currentPhase == 0 || currentPhase == 255))
                {
                    CostSpell();
                    SetCooldown();
                }
            }

            var executionContext = new SpellExecutionContext();
            executionContext.Initialise(this);

            foreach (Spell4EffectsEntry entry in Parameters.SpellInfo.Effects)
                executionContext.AddSpellEffect(entry);

            Execute(executionContext);
        }

        protected void Execute(ISpellExecutionContext executionContext)
        {
            if (!executionContext.GetSpellEffects().Any())
                return;

            status = SpellStatus.Executing;
            log.LogTrace($"Spell {Parameters.SpellInfo.Entry.Id} has started executing.");

            SelectTargets(executionContext);  // First Select Targets
            ExecuteEffects(executionContext); // All Effects are evaluated and executed (after SelectTargets())
            HandleProxies(executionContext);  // Any Proxies that are added by Effects are evaluated and executed (after ExecuteEffects())
            
            if (!executionContext.IsDelayed)
                SendSpellGo();                // Inform the Client once all evaluations are taken place (after Effects & Proxies are executed)

            foreach (ICombatLog combatLog in executionContext.GetCombatLogs())
            {
                Caster.EnqueueToVisible(new ServerCombatLog
                {
                    CombatLog = combatLog
                }, true);
            }
        }

        protected void HandleProxies(ISpellExecutionContext executionContext)
        {
            foreach (IProxy proxy in executionContext.GetProxies())
                proxy.Evaluate();

            foreach (IProxy proxy in executionContext.GetProxies())
                proxy.Cast(Caster, events);
        }

        protected void SetCooldown()
        {
            if (Caster is not IPlayer player)
                return;

            if (Parameters.SpellInfo.Entry.SpellCoolDown != 0u)
                player.SpellManager.SetSpellCooldown(Parameters.SpellInfo, Parameters.SpellInfo.Entry.SpellCoolDown / 1000d, true);
        }

        protected void CostSpell()
        {
            if (Parameters.CharacterSpell?.MaxAbilityCharges > 0)
                Parameters.CharacterSpell.UseCharge();

            for (int i = 0; i < Parameters.SpellInfo.Entry.InnateCostTypes.Length; i++)
            {
                uint innateCostType = Parameters.SpellInfo.Entry.InnateCostTypes[i];
                if (innateCostType == 0)
                    continue;

                Caster.ModifyVital((Vital)innateCostType, Parameters.SpellInfo.Entry.InnateCosts[i] * -1f);
            }
        }

        private IUnitEntity GetExplicitTarget()
        {
            if (Parameters.PrimaryTargetId == 0)
                return Caster;

            return Caster.GetVisible<IUnitEntity>(Parameters.PrimaryTargetId);
        }

        protected virtual void SelectTargets(ISpellExecutionContext executionContext)
        {
            // Add Caster Entity with the appropriate SpellEffectTargetFlags.
            executionContext.TargetCollection.AddTarget(SpellEffectTargetFlags.Caster, Caster);

            // Add Targeted Entity with the appropriate SpellEffectTargetFlags.
            IUnitEntity explicitTarget = GetExplicitTarget();
            if (explicitTarget != null)
                executionContext.TargetCollection.AddTarget(SpellEffectTargetFlags.ExplicitTarget, explicitTarget);

            // TODO: this might not be entirely correct, research this more...
            if (Parameters.SpellInfo.BaseInfo.TargetMechanics.TargetType
                is SpellTargetMechanicType.Self
                or SpellTargetMechanicType.PrimaryTarget)
                return;

            var implicitTargets = new List<ISpellTargetImplicit>();

            // Targeting First Pass: Do Basic Checks to get targets for spell as needed, nearby.
            var targetSelector = LegacyServiceProvider.Provider.GetService<ISpellTargetImplicitSelector>();
            targetSelector.Initialise(Caster, Parameters);
            targetSelector.SelectTargets(implicitTargets);

            InitialiseTelegraphs();

            foreach (ITelegraph telegraph in telegraphs)
            {
                var telegraphFilter = LegacyServiceProvider.Provider.GetService<ISpellTargetImplicitTelegraphFilter>();
                telegraphFilter.Initialise(telegraph, Caster);
                telegraphFilter.Filter(implicitTargets);
            }

            var constraintFilter = LegacyServiceProvider.Provider.GetService<ISpellTargetImplicitConstraintFilter>();
            constraintFilter.Initialise(Parameters.SpellInfo.AoeTargetConstraints);
            constraintFilter.Filter(implicitTargets);

            // add targets...
            foreach (ISpellTargetImplicit implicitTarget in implicitTargets)
                if (implicitTarget.Result == null)
                    executionContext.TargetCollection.AddTarget(SpellEffectTargetFlags.ImplicitTarget, implicitTarget.Entity);
        }

        private void ExecuteEffects(ISpellExecutionContext executionContext)
        {
            foreach (Spell4EffectsEntry effect in executionContext.GetSpellEffects())
                if (CanExecuteEffect(effect))
                    ExecuteEffect(effect, executionContext);
        }

        protected virtual bool CanExecuteEffect(Spell4EffectsEntry spell4EffectsEntry)
        {
            if (disableManager.IsDisabled(DisableType.SpellEffect, spell4EffectsEntry.Id))
                return false;

            if (delayedEffects.TryGetValue(spell4EffectsEntry, out UpdateTimer updateTimer)
                && !updateTimer.HasElapsed)
                return false;

            return true;
        }


        protected virtual void ExecuteEffect(Spell4EffectsEntry spell4EffectsEntry, ISpellExecutionContext executionContext)
        {
            log.LogTrace($"Executing SpellEffect ID {spell4EffectsEntry.Id} ({1 << currentPhase})");

            if (!executionContext.IsDelayed && spell4EffectsEntry.DelayTime > 0)
            {
                delayedEffects.Add(spell4EffectsEntry, new UpdateTimer(TimeSpan.FromMilliseconds(spell4EffectsEntry.DelayTime)));
                log.LogTrace($"Delaying effect {spell4EffectsEntry.Id} execution for {spell4EffectsEntry.DelayTime} ms.");
            }

            foreach (ISpellTarget spellTarget in executionContext.TargetCollection.GetTargets(spell4EffectsEntry.TargetFlags))
            {
                if (!CheckEffectApplyPrerequisites(spell4EffectsEntry, spellTarget.Entity))
                    continue;

                ISpellTargetInfo spellTargetInfo =
                    spellTargetInfoCollection.GetSpellTargetInfo(spellTarget) ??
                    spellTargetInfoCollection.CreateSpellTargetInfo(spellTarget);

                SpellEffectExecutionResult result = spellTargetInfo.Execute(spell4EffectsEntry, executionContext);
                if (result == SpellEffectExecutionResult.Ok)
                {
                    // Track the number of times this effect has fired.
                    // Some spell effects have a limited trigger count per spell cast.
                    executionContext.IncrementEffectTriggerCount(spell4EffectsEntry.Id);
                }
            }
        }

        private bool CheckEffectApplyPrerequisites(Spell4EffectsEntry spell4EffectsEntry, IUnitEntity target)
        {
            if (spell4EffectsEntry.PrerequisiteIdCasterApply > 0
                && !PrerequisiteManager.Instance.Meets(Caster, spell4EffectsEntry.PrerequisiteIdCasterApply, target))
                return false;

            if (spell4EffectsEntry.PrerequisiteIdTargetApply > 0
                && !PrerequisiteManager.Instance.Meets(target, spell4EffectsEntry.PrerequisiteIdTargetApply, Caster))
                return false;

            return true;
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
            spellTargetInfoCollection.Cancel();

            status = SpellStatus.Finishing;

            log.LogTrace($"Spell {Parameters.SpellInfo.Entry.Id} is finishing.");
        }

        private bool PassEntityChecks()
        {
            if (Caster is IPlayer)
                return Parameters.UserInitiatedSpellCast;

            return true;
        }

        protected virtual bool _IsCasting()
        {
            if (Parameters.IsProxy)
                return false;

            if ((Caster is not IPlayer) && status == SpellStatus.Initiating)
                return true;

            return PassEntityChecks();
        }

        protected void SendSpellCastResult(CastResult castResult)
        {
            if (castResult == CastResult.Ok)
                return;

            log.LogTrace($"Spell {Parameters.SpellInfo.Entry.Id} failed to cast {castResult}.");

            if (Caster is IPlayer player && !player.IsLoading)
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

            return Caster.Guid;
        }

        protected void SendSpellStart()
        {
            var spellStart = new ServerSpellStart
            {
                CastingId              = CastingId,
                CasterId               = Caster.Guid,
                PrimaryTargetId        = GetPrimaryTargetId(),
                Spell4Id               = Parameters.SpellInfo.Entry.Id,
                RootSpell4Id           = Parameters.RootSpellInfo?.Entry.Id ?? 0,
                ParentSpell4Id         = Parameters.ParentSpellInfo?.Entry.Id ?? 0,
                FieldPosition          = new Position(Caster.Position),
                Yaw                    = Caster.Rotation.X,
                UserInitiatedSpellCast = Parameters.UserInitiatedSpellCast
            };

            // TODO: Add Proxy Units
            List<IUnitEntity> unitsCasting = new List<IUnitEntity>();
            unitsCasting.Add(Caster);

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
                {
                    spellStart.TelegraphPositionData.Add(new TelegraphPosition
                    {
                        TelegraphId    = (ushort)telegraph.TelegraphDamage.Id,
                        AttachedUnitId = unit.Guid,
                        TargetFlags    = 3,
                        Position       = new Position(telegraph.Position),
                        Yaw            = telegraph.Rotation.X
                    });
                }
            }

            Caster.EnqueueToVisible(spellStart, true);
        }

        private void SendSpellFinish()
        {
            if (status != SpellStatus.Finished)
                return;

            Caster.EnqueueToVisible(new ServerSpellFinish
            {
                ServerUniqueId = CastingId,
            }, true);
        }

        private void SendSpellGo()
        {
            /*if (!targetInfoCollection.HasTargets)
                return;*/

            var serverSpellGo = new ServerSpellGo
            {
                ServerUniqueId     = CastingId,
                PrimaryDestination = new Position(Caster.Position),
                Phase              = currentPhase
            };

            foreach (ISpellTargetInfo targetInfo in spellTargetInfoCollection)
                serverSpellGo.TargetInfoData.Add(targetInfo.Build());

            var unitsCasting = new List<IUnitEntity>
            {
                Caster
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
            {
                foreach (ITelegraph telegraph in telegraphs)
                {
                    serverSpellGo.TelegraphPositionData.Add(new TelegraphPosition
                    {
                        TelegraphId    = (ushort)telegraph.TelegraphDamage.Id,
                        AttachedUnitId = unit.Guid,
                        TargetFlags    = 3,
                        Position       = new Position(telegraph.Position),
                        Yaw            = telegraph.Rotation.X
                    });
                }
            }

            Caster.EnqueueToVisible(serverSpellGo, true);
        }

        public SpellInit BuildSpellInit()
        {
            var spellInit = new SpellInit
            {
                CasterId         = Caster.Guid,
                OriginalTargetId = GetPrimaryTargetId(), // ??
                ServerUniqueId   = CastingId,
                SpellId          = Parameters.SpellInfo.Entry.Id
            };

            foreach (ISpellTargetInfo targetInfo in spellTargetInfoCollection)
                spellInit.TargetInfoData.Add(targetInfo.Build());

            var unitsCasting = new List<IUnitEntity>
            {
                Caster
            };

            foreach (IUnitEntity unit in unitsCasting)
            {
                spellInit.InitialPositionData.Add(new InitialPosition
                {
                    UnitId      = unit.Guid,
                    Position    = new Position(unit.Position),
                    TargetFlags = 3,
                    Yaw         = unit.Rotation.X
                });
            }

            foreach (IUnitEntity unit in unitsCasting)
            {
                foreach (ITelegraph telegraph in telegraphs)
                {
                    spellInit.TelegraphPositionData.Add(new TelegraphPosition
                    {
                        TelegraphId    = (ushort)telegraph.TelegraphDamage.Id,
                        AttachedUnitId = unit.Guid,
                        TargetFlags    = 3,
                        Position       = new Position(telegraph.Position),
                        Yaw            = telegraph.Rotation.X
                    });
                }
            }

            return spellInit;
        }

        private void CheckPersistance(double lastTick)
        {
            if (Parameters.SpellInfo.CasterPersistencePrerequisite == null
                && Parameters.SpellInfo.TargetPersistencePrerequisite == null)
                return;

            persistCheck.Update(lastTick);
            if (!persistCheck.HasElapsed)
                return;

            persistCheck.Reset();

            IUnitEntity target = GetExplicitTarget();
            if (Parameters.SpellInfo.CasterPersistencePrerequisite != null)
            {
                if (!PrerequisiteManager.Instance.Meets(Caster, Parameters.SpellInfo.CasterPersistencePrerequisite.Id, target))
                {
                    Finish();
                    return;
                }
            }

            if (Parameters.SpellInfo.TargetPersistencePrerequisite != null)
            {
                if (target == null)
                {
                    Finish();
                    return;
                }

                if (!PrerequisiteManager.Instance.Meets(target, Parameters.SpellInfo.TargetPersistencePrerequisite.Id, Caster))
                {
                    Finish();
                    return;
                }
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
            return (status == SpellStatus.Executing
                    && spellTargetInfoCollection.IsFinalised
                    && !events.HasPendingEvent
                    && !Parameters.ForceCancelOnly
                    && delayedEffects.Count == 0)
                || status == SpellStatus.Finishing;
        }
    }
}
