using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Abstract.Spell.Target;
using NexusForever.Game.Abstract.Spell.Validator;
using NexusForever.Game.Spell.Event;
using NexusForever.Game.Static.Spell;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Game.Spell.Type
{
    public class SpellClientSideInteraction : Spell
    {
        public override CastMethod CastMethod => CastMethod.ClientSideInteraction;

        #region Dependency Injection

        private readonly ILogger<SpellClientSideInteraction> log;

        public SpellClientSideInteraction(
            ILogger<SpellClientSideInteraction> log,
            ISpellTargetInfoCollection spellTargetInfoCollection,
            IGlobalSpellManager globalSpellManager,
            ICastResultValidatorManager castResultValidatorManager,
            IDisableManager disableManager)
            : base(log, spellTargetInfoCollection, globalSpellManager, castResultValidatorManager, disableManager)
        {
            this.log = log;
        }

        #endregion

        public override bool Cast()
        {
            if (!base.Cast())
                return false;

            double castTime = Parameters.CastTimeOverride > 0 ? Parameters.CastTimeOverride / 1000d : Parameters.SpellInfo.Entry.CastTime / 1000d;
            if ((CastMethod)Parameters.SpellInfo.BaseInfo.Entry.CastMethod != CastMethod.ClientSideInteraction)
                events.EnqueueEvent(new SpellEvent(castTime, SucceedClientInteraction));

            status = SpellStatus.Casting;
            log.LogTrace($"Spell {Parameters.SpellInfo.Entry.Id} has started casting.");
            return true;
        }

        protected override bool _IsCasting()
        {
            return base._IsCasting() && status == SpellStatus.Casting;
        }

        private void SendSpellStartClientInteraction()
        {
            // Shoule we actually emit client interaction events to everyone? - Logs suggest that we only see this packet firing when the client interacts with -something- and is likely only sent to them
            if (Caster is IPlayer player)
            {
                player.Session.EnqueueMessageEncrypted(new ServerSpellStartClientInteraction
                {
                    ClientUniqueId = Parameters.ClientSideInteraction.ClientUniqueId,
                    CastingId = CastingId,
                    CasterId = GetPrimaryTargetId()
                });
            }
        }

        /// <summary>
        /// Used when a <see cref="CSI.ClientSideInteraction"/> succeeds
        /// </summary>
        public void SucceedClientInteraction()
        {
            Execute();

            // the Activate effect reports success, but it can't apply to a target that isn't a unit
            bool activateEffect = Parameters.SpellInfo.Effects.FirstOrDefault(x => x.EffectType == SpellEffectType.Activate) != null;
            bool unitTarget     = Parameters.PrimaryTargetId == 0 || Caster.GetVisible<IGridEntity>(Parameters.PrimaryTargetId) is IUnitEntity;
            if (!activateEffect || !unitTarget)
                Parameters.ClientSideInteraction?.HandleSuccess(this);
        }

        /// <summary>
        /// Used when a <see cref="CSI.ClientSideInteraction"/> fails
        /// </summary>
        public void FailClientInteraction()
        {
            Parameters.ClientSideInteraction?.TriggerFail();

            CancelCast(CastResult.ClientSideInteractionFail);
        }

        protected override void OnStatusChange(SpellStatus previousStatus, SpellStatus status)
        {
            switch (status)
            {
                case SpellStatus.Casting:
                    if (Parameters.ClientSideInteraction.Entry != null)
                        SendSpellStart();
                    else
                        SendSpellStartClientInteraction();
                    break;
            }
        }

        protected override uint GetPrimaryTargetId()
        {
            return Parameters.ClientSideInteraction.Entry != null ? Caster.Guid : Parameters.PrimaryTargetId;
        }
    }
}
