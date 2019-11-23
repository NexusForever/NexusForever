using System;
using System.Collections.Generic;
using System.Linq;
using NexusForever.Shared;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Prerequisite;
using NexusForever.WorldServer.Game.Spell.Event;
using NexusForever.WorldServer.Game.Spell.Static;
using NexusForever.WorldServer.Network.Message.Model;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using NLog;

namespace NexusForever.WorldServer.Game.Spell
{
    public partial class Spell : IUpdate
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public uint CastingId { get; }
        public bool IsCasting => status == SpellStatus.Casting;
        public bool IsFinished => status == SpellStatus.Finished;

        private readonly UnitEntity caster;
        private readonly SpellParameters parameters;
        private SpellStatus status;

        private readonly List<SpellTargetInfo> targets = new();

        private readonly SpellEventManager events = new();

        public Spell(UnitEntity caster, SpellParameters parameters)
        {
            this.caster     = caster;
            this.parameters = parameters;
            CastingId       = GlobalSpellManager.Instance.NextCastingId;
            status          = SpellStatus.Initiating;

            if (parameters.RootSpellInfo == null)
                parameters.RootSpellInfo = parameters.SpellInfo;
        }

        public void Update(double lastTick)
        {
            events.Update(lastTick);

            if (status == SpellStatus.Executing && !events.HasPendingEvent)
            {
                // spell effects have finished executing
                status = SpellStatus.Finished;
                log.Trace($"Spell {parameters.SpellInfo.Entry.Id} has finished.");

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

            log.Trace($"Spell {parameters.SpellInfo.Entry.Id} has started initating.");

            CastResult result = CheckCast();
            if (result != CastResult.Ok)
            {
                SendSpellCastResult(result);
                return;
            }

            if (caster is Player player)
                if (parameters.SpellInfo.GlobalCooldown != null)
                    player.SpellManager.SetGlobalSpellCooldown(parameters.SpellInfo.GlobalCooldown.CooldownTime / 1000d);

            SendSpellStart();

            // enqueue spell to be executed after cast time
            events.EnqueueEvent(new SpellEvent(parameters.SpellInfo.Entry.CastTime / 1000d, Execute));
            status = SpellStatus.Casting;

            log.Trace($"Spell {parameters.SpellInfo.Entry.Id} has started casting.");
        }

        private CastResult CheckCast()
        {
            CastResult preReqCheck = CheckPrerequisites();
            if (preReqCheck != CastResult.Ok)
                return preReqCheck;

            CastResult ccResult = CheckCCConditions();
            if (ccResult != CastResult.Ok)
                return ccResult;

            if (caster is Player player)
            {
                if (player.SpellManager.GetSpellCooldown(parameters.SpellInfo.Entry.Id) > 0d)
                    return CastResult.SpellCooldown;

                // this isn't entirely correct, research GlobalCooldownEnum
                if (parameters.SpellInfo.Entry.GlobalCooldownEnum == 0
                    && player.SpellManager.GetGlobalSpellCooldown() > 0d)
                    return CastResult.SpellGlobalCooldown;

                if (parameters.CharacterSpell?.MaxAbilityCharges > 0 && parameters.CharacterSpell?.AbilityCharges == 0)
                    return CastResult.SpellNoCharges;
                    
                for (int i = 0; i < parameters.SpellInfo.Entry.CasterInnateRequirements.Length; i++)
                {
                    uint innateRequirement = parameters.SpellInfo.Entry.CasterInnateRequirements[i];
                    if (innateRequirement == 0)
                        continue;

                    switch (parameters.SpellInfo.Entry.CasterInnateRequirementEval[i])
                    {
                        case 2:
                            if (caster.GetVitalValue((Vital)innateRequirement) < parameters.SpellInfo.Entry.CasterInnateRequirementValues[i])
                                return GlobalSpellManager.Instance.GetFailedCastResultForVital((Vital)innateRequirement);
                            break;
                    }
                }

                for (int i = 0; i < parameters.SpellInfo.Entry.InnateCostTypes.Length; i++)
                {
                    uint innateCostType = parameters.SpellInfo.Entry.InnateCostTypes[i];
                    if (innateCostType == 0)
                        continue;

                    if (caster.GetVitalValue((Vital)innateCostType) < parameters.SpellInfo.Entry.InnateCosts[i])
                        return GlobalSpellManager.Instance.GetFailedCastResultForVital((Vital)innateCostType);
                }
            }

            return CastResult.Ok;
        }

        private CastResult CheckPrerequisites()
        {
            // TODO: Remove below line and evaluate PreReq's for Non-Player Entities
            if (!(caster is Player player))
                return CastResult.Ok;

            if (parameters.SpellInfo.CasterCastPrerequisite != null && !CheckRunnerOverride(player))
            {
                if (!PrerequisiteManager.Instance.Meets(player, parameters.SpellInfo.CasterCastPrerequisite.Id))
                    return CastResult.PrereqCasterCast;
            }

            // not sure if this should be for explicit and/or implicit targets
            if (parameters.SpellInfo.TargetCastPrerequisites != null)
            {
            }

            // this probably isn't the correct place, name implies this should be constantly checked
            if (parameters.SpellInfo.CasterPersistencePrerequisites != null)
            {
            }

            if (parameters.SpellInfo.TargetPersistencePrerequisites != null)
            {
            }

            return CastResult.Ok;
        }

        private bool CheckRunnerOverride(Player player)
        {
            foreach (PrerequisiteEntry runnerPrereq in parameters.SpellInfo.PrerequisiteRunners)
                if (PrerequisiteManager.Instance.Meets(player, runnerPrereq.Id))
                    return true;

            return false;
        }

        private CastResult CheckCCConditions()
        {
            // TODO: this just looks like a mask for CCState enum
            if (parameters.SpellInfo.CasterCCConditions != null)
            {
            }

            // not sure if this should be for explicit and/or implicit targets
            if (parameters.SpellInfo.TargetCCConditions != null)
            {
            }

            return CastResult.Ok;
        }

        /// <summary>
        /// Cancel cast with supplied <see cref="CastResult"/>.
        /// </summary>
        public void CancelCast(CastResult result)
        {
            if (status != SpellStatus.Casting)
                throw new InvalidOperationException();

            if (caster is Player player && !player.IsLoading)
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

            log.Trace($"Spell {parameters.SpellInfo.Entry.Id} cast was cancelled.");
        }

        private void Execute()
        {
            status = SpellStatus.Executing;
            log.Trace($"Spell {parameters.SpellInfo.Entry.Id} has started executing.");

            if (caster is Player player)
                if (parameters.SpellInfo.Entry.SpellCoolDown != 0u)
                    player.SpellManager.SetSpellCooldown(parameters.SpellInfo.Entry.Id, parameters.SpellInfo.Entry.SpellCoolDown / 1000d);

            SelectTargets();
            ExecuteEffects();
            CostSpell();
            ChargeSpellCost();

            SendSpellGo();
        }

        private void CostSpell()
        {
            if (parameters.CharacterSpell?.MaxAbilityCharges > 0)
                parameters.CharacterSpell.UseCharge();
        }

        private void SelectTargets()
        {
            targets.Add(new SpellTargetInfo(SpellEffectTargetFlags.Caster, caster));
            foreach (TelegraphDamageEntry telegraphDamageEntry in parameters.SpellInfo.Telegraphs)
            {
                var telegraph = new Telegraph(telegraphDamageEntry, caster, caster.Position, caster.Rotation);
                foreach (UnitEntity entity in telegraph.GetTargets())
                    targets.Add(new SpellTargetInfo(SpellEffectTargetFlags.Telegraph, entity));
            }
        }

        private void ExecuteEffects()
        {
            foreach (Spell4EffectsEntry spell4EffectsEntry in parameters.SpellInfo.Effects)
            {
                // select targets for effect
                List<SpellTargetInfo> effectTargets = targets
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

        private void ChargeSpellCost()
        {
            for (int i = 0; i < parameters.SpellInfo.Entry.InnateCostTypes.Length; i++)
            {
                uint innateCostType = parameters.SpellInfo.Entry.InnateCostTypes[i];
                if (innateCostType == 0)
                    continue;

                caster.ModifyVital((Vital)innateCostType, parameters.SpellInfo.Entry.InnateCosts[i] * -1f);
            }
        }

        public bool IsMovingInterrupted()
        {
            // TODO: implement correctly
            return parameters.SpellInfo.Entry.CastTime > 0;
        }

        private void SendSpellCastResult(CastResult castResult)
        {
            if (castResult == CastResult.Ok)
                return;

            log.Trace($"Spell {parameters.SpellInfo.Entry.Id} failed to cast {castResult}.");

            if (caster is Player player && !player.IsLoading)
            {
                player.Session.EnqueueMessageEncrypted(new ServerSpellCastResult
                {
                    Spell4Id   = parameters.SpellInfo.Entry.Id,
                    CastResult = castResult
                });
            }
        }

        private void SendSpellStart()
        {
            caster.EnqueueToVisible(new ServerSpellStart
            {
                CastingId              = CastingId,
                CasterId               = caster.Guid,
                PrimaryTargetId        = caster.Guid,
                Spell4Id               = parameters.SpellInfo.Entry.Id,
                RootSpell4Id           = parameters.RootSpellInfo?.Entry.Id ?? 0,
                ParentSpell4Id         = parameters.ParentSpellInfo?.Entry.Id ?? 0,
                FieldPosition          = new Position(caster.Position),
                UserInitiatedSpellCast = parameters.UserInitiatedSpellCast
            }, true);
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
            var serverSpellGo = new ServerSpellGo
            {
                ServerUniqueId     = CastingId,
                PrimaryDestination = new Position(caster.Position),
                Phase              = -1
            };

            foreach (SpellTargetInfo targetInfo in targets
                .Where(t => t.Effects.Count > 0))
            {
                var networkTargetInfo = new TargetInfo
                {
                    UnitId        = targetInfo.Entity.Guid,
                    TargetFlags   = 1,
                    InstanceCount = 1,
                    CombatResult  = CombatResult.Hit
                };

                foreach (SpellTargetInfo.SpellTargetEffectInfo targetEffectInfo in targetInfo.Effects)
                {
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
                            CombatResult       = CombatResult.Hit,
                            DamageType         = targetEffectInfo.Damage.DamageType
                        };
                    }

                    networkTargetInfo.EffectInfoData.Add(networkTargetEffectInfo);
                }

                serverSpellGo.TargetInfoData.Add(networkTargetInfo);
            }

            caster.EnqueueToVisible(serverSpellGo, true);
        }

        private void SendRemoveBuff(uint unitId)
        {
            if (!parameters.SpellInfo.BaseInfo.HasIcon)
                throw new InvalidOperationException();

            caster.EnqueueToVisible(new ServerSpellBuffRemove
            {
                CastingId = CastingId,
                CasterId  = unitId
            }, true);
        }
    }
}
