﻿using System;
using System.Collections.Generic;
using System.Linq;
using NexusForever.Shared;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Spell.Event;
using NexusForever.WorldServer.Game.Spell.Static;
using NexusForever.WorldServer.Network.Message.Model;
using NLog;

namespace NexusForever.WorldServer.Game.Spell
{
    public partial class Spell : IUpdate
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public bool IsCasting => status == SpellStatus.Casting;
        public bool IsFinished => status == SpellStatus.Finished;

        private readonly UnitEntity caster;
        private readonly SpellParameters parameters;
        private readonly uint castingId;
        private SpellStatus status;

        private readonly List<SpellTargetInfo> targets = new List<SpellTargetInfo>();

        private readonly SpellEventManager events = new SpellEventManager();

        public Spell(UnitEntity caster, SpellParameters parameters)
        {
            this.caster     = caster;
            this.parameters = parameters;
            castingId       = GlobalSpellManager.NextCastingId;
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
                log.Trace($"Spell {parameters.SpellInfo.Entry.Id} failed to cast {result}.");

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
            if (!CheckPrerequisites())
                return CastResult.SpellPreRequisites;

            CastResult ccResult = CheckCCConditions();
            if (ccResult != CastResult.Ok)
                return ccResult;

            if (caster is Player player)
            {
                if (player.SpellManager.GetSpellCooldown(parameters.SpellInfo.BaseInfo.Entry.Id) > 0d)
                    return CastResult.SpellCooldown;

                // this isn't entirely correct, research GlobalCooldownEnum
                if (parameters.SpellInfo.Entry.GlobalCooldownEnum == 0
                    && player.SpellManager.GetGlobalSpellCooldown() > 0d)
                    return CastResult.SpellGlobalCooldown;
            }

            return CastResult.Ok;
        }

        private bool CheckPrerequisites()
        {
            // TODO
            if (parameters.SpellInfo.CasterCastPrerequisites != null)
            {
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

            return true;
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
                    ServerUniqueId = castingId,
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
                    player.SpellManager.SetSpellCooldown(parameters.SpellInfo.BaseInfo.Entry.Id, parameters.SpellInfo.Entry.SpellCoolDown / 1000d);

            SelectTargets();
            ExecuteEffects();

            SendSpellGo();
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

                SpellEffectDelegate handler = GlobalSpellManager.GetEffectHandler((SpellEffectType)spell4EffectsEntry.EffectType);
                if (handler == null)
                    log.Warn($"Unhandled spell effect {(SpellEffectType)spell4EffectsEntry.EffectType}");
                else
                {
                    uint effectId = GlobalSpellManager.NextEffectId;
                    foreach (SpellTargetInfo effectTarget in effectTargets)
                    {
                        var info = new SpellTargetInfo.SpellTargetEffectInfo(effectId, spell4EffectsEntry);
                        effectTarget.Effects.Add(info);

                        handler.Invoke(this, effectTarget.Entity, info);
                    }
                }
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
                CastingId              = castingId,
                CasterId               = caster.Guid,
                PrimaryTargetId        = caster.Guid,
                Spell4Id               = parameters.SpellInfo.Entry.Id,
                RootSpell4Id           = parameters.RootSpellInfo?.Entry.Id ?? 0,
                ParentSpell4Id         = parameters.ParentSpellInfo?.Entry.Id ?? 0,
                FieldPosition          = new Position(caster.Position),
                UserInitiatedSpellCast = parameters.UserInitiatedSpellCast
            }, true);
        }

        private void SendSpellGo()
        {
            var serverSpellGo = new ServerSpellGo
            {
                ServerUniqueId     = castingId,
                PrimaryDestination = new Position(caster.Position),
                Phase              = -1
            };

            foreach (SpellTargetInfo targetInfo in targets
                .Where(t => t.Effects.Count > 0))
            {
                var networkTargetInfo = new ServerSpellGo.TargetInfo
                {
                    UnitId        = targetInfo.Entity.Guid,
                    TargetFlags   = 1,
                    InstanceCount = 1,
                    CombatResult  = CombatResult.Hit
                };

                foreach (SpellTargetInfo.SpellTargetEffectInfo targetEffectInfo in targetInfo.Effects)
                {
                    var networkTargetEffectInfo = new ServerSpellGo.TargetInfo.EffectInfo
                    {
                        Spell4EffectId = targetEffectInfo.Entry.Id,
                        EffectUniqueId = targetEffectInfo.EffectId,
                        TimeRemaining  = -1
                    };

                    if (targetEffectInfo.Damage != null)
                    {
                        networkTargetEffectInfo.InfoType = 1;
                        networkTargetEffectInfo.DamageDescriptionData = new ServerSpellGo.TargetInfo.EffectInfo.DamageDescription
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
    }
}
