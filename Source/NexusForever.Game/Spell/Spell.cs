using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Abstract.Spell.Event;
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
using NLog;

namespace NexusForever.Game.Spell
{
    public partial class Spell : ISpell
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public ISpellParameters Parameters { get; }
        public uint CastingId { get; }
        public bool IsCasting => status == SpellStatus.Casting;
        public bool IsFinished => status == SpellStatus.Finished;

        public IUnitEntity Caster { get; }

        private SpellStatus status;

        private readonly List<ISpellTargetInfo> targets = new();
        private readonly List<ITelegraph> telegraphs = new();

        private readonly ISpellEventManager events = new SpellEventManager();

        private IScriptCollection scriptCollection;

        public Spell(IUnitEntity caster, ISpellParameters parameters)
        {
            Caster     = caster;
            Parameters = parameters;
            CastingId  = GlobalSpellManager.Instance.NextCastingId;
            status     = SpellStatus.Initiating;

            parameters.RootSpellInfo ??= parameters.SpellInfo;

            scriptCollection = ScriptManager.Instance.InitialiseOwnedScripts<ISpell>(this, parameters.SpellInfo.Entry.Id);
        }

        public void Dispose()
        {
            if (scriptCollection != null)
                ScriptManager.Instance.Unload(scriptCollection);

            scriptCollection = null;
        }

        public void Update(double lastTick)
        {
            scriptCollection.Invoke<IUpdate>(s => s.Update(lastTick));

            events.Update(lastTick);

            if (status == SpellStatus.Executing && !events.HasPendingEvent)
            {
                // spell effects have finished executing
                status = SpellStatus.Finished;
                log.Trace($"Spell {Parameters.SpellInfo.Entry.Id} has finished.");

                // TODO: add a timer to count down on the Effect before sending the finish - sending the finish will e.g. wear off the buff
                //SendSpellFinish();
            }
        }

        /// <summary>
        /// Begin cast, checking prerequisites before initiating.
        /// </summary>
        public void Cast()
        {
            if (status != SpellStatus.Initiating)
                throw new InvalidOperationException();

            log.Trace($"Spell {Parameters.SpellInfo.Entry.Id} has started initating.");

            CastResult result = CheckCast();
            if (result != CastResult.Ok)
            {
                SendSpellCastResult(result);
                return;
            }

            if (Caster is IPlayer player)
                if (Parameters.SpellInfo.GlobalCooldown != null)
                    player.SpellManager.SetGlobalSpellCooldown(Parameters.SpellInfo.GlobalCooldown.CooldownTime / 1000d);

            // It's assumed that non-player entities will be stood still to cast (most do). 
            // TODO: There are a handful of telegraphs that are attached to moving units (specifically rotating units) which this needs to be updated to account for.
            if (Caster is not IPlayer)
                InitialiseTelegraphs();

            SendSpellStart();

            // enqueue spell to be executed after cast time
            events.EnqueueEvent(new SpellEvent(Parameters.SpellInfo.Entry.CastTime / 1000d, Execute));
            status = SpellStatus.Casting;

            log.Trace($"Spell {Parameters.SpellInfo.Entry.Id} has started casting.");
        }

        private CastResult CheckCast()
        {
            CastResult preReqCheck = CheckPrerequisites();
            if (preReqCheck != CastResult.Ok)
                return preReqCheck;

            CastResult ccResult = CheckCCConditions();
            if (ccResult != CastResult.Ok)
                return ccResult;

            if (Caster is IPlayer player)
            {
                if (player.SpellManager.GetSpellCooldown(Parameters.SpellInfo.Entry.Id) > 0d)
                    return CastResult.SpellCooldown;

                // this isn't entirely correct, research GlobalCooldownEnum
                if (Parameters.SpellInfo.Entry.GlobalCooldownEnum == 0
                    && player.SpellManager.GetGlobalSpellCooldown() > 0d)
                    return CastResult.SpellGlobalCooldown;

                if (Parameters.CharacterSpell?.MaxAbilityCharges > 0 && Parameters.CharacterSpell?.AbilityCharges == 0)
                    return CastResult.SpellNoCharges;
            }

            return CastResult.Ok;
        }

        private CastResult CheckPrerequisites()
        {
            // TODO: Remove below line and evaluate PreReq's for Non-Player Entities
            if (Caster is not IPlayer player)
                return CastResult.Ok;

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

        private bool CheckRunnerOverride(IPlayer player)
        {
            foreach (PrerequisiteEntry runnerPrereq in Parameters.SpellInfo.PrerequisiteRunners)
                if (PrerequisiteManager.Instance.Meets(player, runnerPrereq.Id))
                    return true;

            return false;
        }

        private CastResult CheckCCConditions()
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

        private void InitialiseTelegraphs()
        {
            telegraphs.Clear();
            foreach (TelegraphDamageEntry telegraphDamageEntry in Parameters.SpellInfo.Telegraphs)
                telegraphs.Add(new Telegraph(telegraphDamageEntry, Caster, Caster.Position, Caster.Rotation));
        }

        /// <summary>
        /// Cancel cast with supplied <see cref="CastResult"/>.
        /// </summary>
        public void CancelCast(CastResult result)
        {
            if (status != SpellStatus.Casting)
                throw new InvalidOperationException();

            if (Caster is IPlayer player && !player.IsLoading)
            {
                player.Session.EnqueueMessageEncrypted(new Server07F9
                {
                    ServerUniqueId = CastingId,
                    CastResult     = result,
                    CancelCast     = true
                });
            }

            events.CancelEvents();
            status = SpellStatus.Executing;

            log.Trace($"Spell {Parameters.SpellInfo.Entry.Id} cast was cancelled.");
        }

        private void Execute()
        {
            status = SpellStatus.Executing;
            log.Trace($"Spell {Parameters.SpellInfo.Entry.Id} has started executing.");

            if (Caster is IPlayer player)
                if (Parameters.SpellInfo.Entry.SpellCoolDown != 0u)
                    player.SpellManager.SetSpellCooldown(Parameters.SpellInfo.Entry.Id, Parameters.SpellInfo.Entry.SpellCoolDown / 1000d);

            SelectTargets();
            ExecuteEffects();
            CostSpell();

            SendSpellGo();
        }

        private void CostSpell()
        {
            if (Parameters.CharacterSpell?.MaxAbilityCharges > 0)
                Parameters.CharacterSpell.UseCharge();
        }

        private void SelectTargets()
        {
            targets.Add(new SpellTargetInfo(SpellEffectTargetFlags.Caster, Caster));

            if (Parameters.PrimaryTargetId != 0)
            {
                IUnitEntity primaryTargetEntity = Caster.GetVisible<IUnitEntity>(Parameters.PrimaryTargetId);
                if (primaryTargetEntity != null)
                    targets.Add(new SpellTargetInfo(SpellEffectTargetFlags.Target, primaryTargetEntity));
            }

            if (Caster is IPlayer)
                InitialiseTelegraphs();

            foreach (ITelegraph telegraph in telegraphs)
            {
                foreach (IUnitEntity entity in telegraph.GetTargets())
                    targets.Add(new SpellTargetInfo(SpellEffectTargetFlags.Telegraph, entity));
            }
        }

        private void ExecuteEffects()
        {
            foreach (Spell4EffectsEntry spell4EffectsEntry in Parameters.SpellInfo.Effects)
            {
                // select targets for effect
                List<ISpellTargetInfo> effectTargets = targets
                    .Where(t => (t.Flags & (SpellEffectTargetFlags)spell4EffectsEntry.TargetFlags) != 0)
                    .ToList();

                SpellEffectDelegate handler = GlobalSpellManager.Instance.GetEffectHandler((SpellEffectType)spell4EffectsEntry.EffectType);
                if (handler == null)
                    log.Warn($"Unhandled spell effect {(SpellEffectType)spell4EffectsEntry.EffectType}");
                else
                {
                    uint effectId = GlobalSpellManager.Instance.NextEffectId;
                    foreach (SpellTargetInfo effectTarget in effectTargets)
                    {
                        var info = new SpellTargetInfo.SpellTargetEffectInfo(effectId, spell4EffectsEntry);
                        effectTarget.Effects.Add(info);

                        // TODO: if there is an unhandled exception in the handler, there will be an infinite loop on Execute()
                        handler.Invoke(this, effectTarget.Entity, info);
                    }
                }
            }
        }

        public bool IsMovingInterrupted()
        {
            // TODO: implement correctly
            return Parameters.SpellInfo.Entry.CastTime > 0;
        }

        private void SendSpellCastResult(CastResult castResult)
        {
            if (castResult == CastResult.Ok)
                return;

            log.Trace($"Spell {Parameters.SpellInfo.Entry.Id} failed to cast {castResult}.");

            if (Caster is IPlayer player && !player.IsLoading)
            {
                player.Session.EnqueueMessageEncrypted(new ServerSpellCastResult
                {
                    Spell4Id   = Parameters.SpellInfo.Entry.Id,
                    CastResult = castResult
                });
            }
        }

        private void SendSpellStart()
        {
            var spellStart = new ServerSpellStart
            {
                CastingId              = CastingId,
                CasterId               = Caster.Guid,
                PrimaryTargetId        = Caster.Guid,
                Spell4Id               = Parameters.SpellInfo.Entry.Id,
                RootSpell4Id           = Parameters.RootSpellInfo?.Entry.Id ?? 0,
                ParentSpell4Id         = Parameters.ParentSpellInfo?.Entry.Id ?? 0,
                FieldPosition          = new Position(Caster.Position),
                Yaw                    = Caster.Rotation.X,
                UserInitiatedSpellCast = Parameters.UserInitiatedSpellCast,
                InitialPositionData    = new List<ServerSpellStart.InitialPosition>(),
                TelegraphPositionData  = new List<ServerSpellStart.TelegraphPosition>()
            };

            var unitsCasting = new List<IUnitEntity>();
            if (Parameters.PrimaryTargetId > 0)
                unitsCasting.Add(Caster.GetVisible<IUnitEntity>(Parameters.PrimaryTargetId));
            else
                unitsCasting.Add(Caster);

            foreach (IUnitEntity unit in unitsCasting)
            {
                spellStart.InitialPositionData.Add(new ServerSpellStart.InitialPosition
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
                    spellStart.TelegraphPositionData.Add(new ServerSpellStart.TelegraphPosition
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
            List<ServerCombatLog> combatLogs = new List<ServerCombatLog>();

            var serverSpellGo = new ServerSpellGo
            {
                ServerUniqueId     = CastingId,
                PrimaryDestination = new Position(Caster.Position),
                Phase              = -1
            };

            foreach (ISpellTargetInfo targetInfo in targets
                .Where(t => t.Effects.Count > 0))
            {
                if (!targetInfo.Effects.Any(x => x.DropEffect == false))
                {
                    combatLogs.AddRange(targetInfo.Effects.SelectMany(i => i.CombatLogs));
                    continue;
                }

                var networkTargetInfo = new TargetInfo
                {
                    UnitId        = targetInfo.Entity.Guid,
                    TargetFlags   = 1,
                    InstanceCount = 1,
                    CombatResult  = CombatResult.Hit
                };

                foreach (ISpellTargetEffectInfo targetEffectInfo in targetInfo.Effects)
                {
                    if (targetEffectInfo.DropEffect)
                    {
                        combatLogs.AddRange(targetEffectInfo.CombatLogs);
                        continue;
                    }

                    if ((SpellEffectType)targetEffectInfo.Entry.EffectType == SpellEffectType.Proxy)
                        continue;

                    var networkTargetEffectInfo = new TargetInfo.EffectInfo
                    {
                        Spell4EffectId = targetEffectInfo.Entry.Id,
                        EffectUniqueId = targetEffectInfo.EffectId,
                        TimeRemaining  = -1
                    };

                    if (targetEffectInfo.Damage != null)
                    {
                        networkTargetEffectInfo.InfoType = 1;
                        networkTargetEffectInfo.DamageDescriptionData = new TargetInfo.EffectInfo.DamageDescription
                        {
                            RawDamage          = targetEffectInfo.Damage.RawDamage,
                            RawScaledDamage    = targetEffectInfo.Damage.RawScaledDamage,
                            AbsorbedAmount     = targetEffectInfo.Damage.AbsorbedAmount,
                            ShieldAbsorbAmount = targetEffectInfo.Damage.ShieldAbsorbAmount,
                            AdjustedDamage     = targetEffectInfo.Damage.AdjustedDamage,
                            OverkillAmount     = targetEffectInfo.Damage.OverkillAmount,
                            KilledTarget       = targetEffectInfo.Damage.KilledTarget,
                            CombatResult       = targetEffectInfo.Damage.CombatResult,
                            DamageType         = targetEffectInfo.Damage.DamageType
                        };
                    }

                    networkTargetInfo.EffectInfoData.Add(networkTargetEffectInfo);

                    combatLogs.AddRange(targetEffectInfo.CombatLogs);
                }

                serverSpellGo.TargetInfoData.Add(networkTargetInfo);
            }

            var unitsCasting = new List<IUnitEntity>
            {
                Caster
            };

            foreach (IUnitEntity unit in unitsCasting)
            {
                serverSpellGo.InitialPositionData.Add(new InitialPosition
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

            foreach (ServerCombatLog combatLog in combatLogs)
                Caster.EnqueueToVisible(combatLog, true);

            Caster.EnqueueToVisible(serverSpellGo, true);

        }

        private void SendRemoveBuff(uint unitId)
        {
            if (!Parameters.SpellInfo.BaseInfo.HasIcon)
                throw new InvalidOperationException();

            Caster.EnqueueToVisible(new ServerSpellBuffRemove
            {
                CastingId = CastingId,
                CasterId  = unitId
            }, true);
        }
    }
}
