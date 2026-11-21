using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Abstract.Spell.Info;
using NexusForever.Game.Abstract.Spell.Target;
using NexusForever.Game.Abstract.Spell.Validator;
using NexusForever.Game.Static.Spell;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model.Spell;

namespace NexusForever.Game.Spell.Type
{
    public abstract class SpellThreshold : Spell
    {
        public bool HasThresholdToCast => Parameters.SpellInfo.Thresholds.Count > 0 && thresholdValue < thresholdMax || thresholdSpells.Count > 0;

        private readonly List<ISpell> thresholdSpells = [];
        private double holdDuration;
        protected uint totalThresholdTimer;
        protected uint thresholdMax;
        protected uint thresholdValue;

        #region Dependency Injection

        private readonly ILogger log;
        private readonly ISpellFactory spellFactory;

        public SpellThreshold(
            ILogger log,
            ISpellTargetInfoCollection spellTargetInfoCollection,
            IGlobalSpellManager globalSpellManager,
            ICastResultValidatorManager castResultValidatorManager,
            IDisableManager disableManager,
            ISpellFactory spellFactory)
            : base(log, spellTargetInfoCollection, globalSpellManager, castResultValidatorManager, disableManager)
        {
            this.log          = log;
            this.spellFactory = spellFactory;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public override void Initialise(IUnitEntity caster, ISpellParameters parameters)
        {
            base.Initialise(caster, parameters);

            thresholdMax = (uint)parameters.SpellInfo.Thresholds.Count;
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public override void Update(double lastTick)
        {
            base.Update(lastTick);

            if (status == SpellStatus.Initiating)
                return;

            // For Threshold Spells, a SpellStatus of Waiting is used to more accurately check state.
            if (status == SpellStatus.Executing && HasThresholdToCast)
                status = SpellStatus.Waiting;

            if (status == SpellStatus.Waiting && CastMethod == CastMethod.ChargeRelease)
            {
                holdDuration += lastTick;

                // For Charge+Hold Spells, they have a maximum time that can be held before the effect will fire. Execute effect at maximum time.
                // This will fire the Child spell, and then clean up this spell in the rest of this loop.
                if (holdDuration >= totalThresholdTimer)
                    HandleThresholdCast();
            }

            // Update all Child Spells that are triggered from Execute thresholds.
            thresholdSpells.ForEach(s => s.Update(lastTick));
            thresholdSpells.ForEach(s => s.LateUpdate(lastTick));
            if (status == SpellStatus.Waiting && HasThresholdToCast)
            {
                // Clean up any finished Child Spells because this Spell cannot finish until it has.
                foreach (Spell thresholdSpell in thresholdSpells.ToList())
                    if (thresholdSpell.IsFinished)
                        thresholdSpells.Remove(thresholdSpell);
            }
        }

        public override bool Cast()
        {
            if (status == SpellStatus.Waiting)
                return HandleThresholdCast();

            if (!base.Cast())
                return false;

            return true;
        }

        protected virtual void Execute()
        {

            if ((currentPhase == 0 || currentPhase == 255) && !HasThresholdToCast && CastMethod != CastMethod.ChargeRelease)
            {
                CostSpell();
                SetCooldown();
            }

            base.Execute(false);

            // TODO: Confirm whether RapidTap spells cancel another out, and add logic as necessary

            if (Parameters.SpellInfo.Entry.ThresholdTime > 0)
                SendThresholdStart();

            if (Parameters.ThresholdValue > 0 && Parameters.RootSpellInfo.Thresholds.Count > 1)
                SendThresholdUpdate();
        }

        private bool HandleThresholdCast()
        {
            if (status != SpellStatus.Waiting)
                throw new InvalidOperationException();

            if (Parameters.SpellInfo.Thresholds.Count == 0)
                throw new InvalidOperationException();

            CastResult result = CheckCast();
            if (result != CastResult.Ok)
            {
                if (CastMethod == CastMethod.RapidTap && result != CastResult.PrereqCasterCast)
                {
                    if (Caster is IPlayer player)
                        player.SpellManager.SetAsContinuousCast(null);

                    SendSpellCastResult(result);
                    return false;
                }
            }

            ISpell thresholdSpell = InitialiseThresholdSpell();
            thresholdSpell.Cast();
            thresholdSpells.Add(thresholdSpell);

            switch (CastMethod)
            {
                case CastMethod.ChargeRelease:
                    SetCooldown();
                    thresholdValue = thresholdMax;
                    status = SpellStatus.Finishing;
                    break;
                case CastMethod.RapidTap:
                    thresholdValue++;
                    break;
            }

            return true;
        }

        private ISpell InitialiseThresholdSpell()
        {
            if (Parameters.SpellInfo.Thresholds.Count == 0)
                return null;

            (ISpellInfo spellInfo, Spell4ThresholdsEntry thresholdsEntry) = Parameters.SpellInfo.GetThresholdSpellInfo((int)thresholdValue);
            if (spellInfo == null || thresholdsEntry == null)
                throw new InvalidOperationException($"{spellInfo} or {thresholdsEntry} is null!");

            ISpell thresholdSpell = spellFactory.CreateSpell(spellInfo.BaseInfo.Entry.CastMethod);
            if (thresholdSpell == null)
                throw new InvalidOperationException();

            thresholdSpell.Initialise(Caster, new SpellParameters
            {
                SpellInfo = spellInfo,
                ParentSpellInfo = Parameters.SpellInfo,
                RootSpellInfo = Parameters.SpellInfo,
                UserInitiatedSpellCast = Parameters.UserInitiatedSpellCast,
                ThresholdValue = thresholdsEntry.OrderIndex + 1,
                IsProxy = CastMethod == CastMethod.ChargeRelease
            });

            log.LogTrace($"Added Child Spell {thresholdSpell.Spell4Id} with casting ID {thresholdSpell.CastingId} to parent casting ID {CastingId}");

            return thresholdSpell;
        }

        public override void CancelCast(CastResult result)
        {
            if (!IsCasting && !HasThresholdToCast)
                return;

            if (HasThresholdToCast && thresholdSpells.Count > 0)
                if (thresholdSpells[0].IsCasting)
                {
                    thresholdSpells[0].CancelCast(result);
                    return;
                }

            base.CancelCast(result);
        }

        public override void Finish()
        {
            if (status == SpellStatus.Finished)
                return;

            thresholdValue = thresholdMax;
            base.Finish();
        }

        private void SendThresholdStart()
        {
            if (Caster is IPlayer player)
                player.Session.EnqueueMessageEncrypted(new ServerSpellThresholdStart
                {
                    Spell4Id = Parameters.SpellInfo.Entry.Id,
                    RootSpell4Id = Parameters.RootSpellInfo?.Entry.Id ?? 0,
                    ParentSpell4Id = Parameters.ParentSpellInfo?.Entry.Id ?? 0,
                    ServerUniqueId = CastingId
                });
        }

        protected void SendThresholdUpdate()
        {
            if (Caster is IPlayer player)
                player.Session.EnqueueMessageEncrypted(new ServerSpellThresholdUpdate
                {
                    Spell4Id = Parameters.ParentSpellInfo?.Entry.Id ?? Spell4Id,
                    CurrentThreshold = Parameters.ThresholdValue > 0 ? (byte)Parameters.ThresholdValue : (byte)thresholdValue
                });
        }

        protected override bool CanFinish()
        {
            return status == SpellStatus.Waiting && !HasThresholdToCast || base.CanFinish();
        }

        protected override void OnStatusChange(SpellStatus previousStatus, SpellStatus status)
        {
            base.OnStatusChange(previousStatus, status);

            if (status == SpellStatus.Finished && Caster is IPlayer player)
            {
                // Clear any Threshold information sent to the caster.
                if (thresholdMax > 0)
                {
                    player.Session.EnqueueMessageEncrypted(new ServerSpellThresholdClear
                    {
                        Spell4Id = Spell4Id
                    });

                    if (CastMethod != CastMethod.ChargeRelease)
                        SetCooldown();
                }
            }
        }
    }
}
