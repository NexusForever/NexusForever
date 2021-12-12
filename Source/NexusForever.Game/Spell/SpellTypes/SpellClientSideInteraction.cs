using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Spell.Event;
using NexusForever.Game.Static.Spell;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Static;
using NLog;

namespace NexusForever.Game.Spell
{
    [SpellType(CastMethod.ClientSideInteraction)]
    public partial class SpellClientSideInteraction : Spell, ISpellType
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private static CastMethod castMethod = CastMethod.ClientSideInteraction;

        public SpellClientSideInteraction(IUnitEntity caster, ISpellParameters parameters) 
            : base(caster, parameters, castMethod)
        {
        }

        public override bool Cast()
        {
            if (!base.Cast())
                return false;

            double castTime = Parameters.CastTimeOverride > 0 ? Parameters.CastTimeOverride / 1000d : Parameters.SpellInfo.Entry.CastTime / 1000d;
            if ((CastMethod)Parameters.SpellInfo.BaseInfo.Entry.CastMethod != CastMethod.ClientSideInteraction)
                events.EnqueueEvent(new SpellEvent(castTime, SucceedClientInteraction));

            status = SpellStatus.Casting;
            log.Trace($"Spell {Parameters.SpellInfo.Entry.Id} has started casting.");
            return true;
        }

        protected override bool _IsCasting()
        {
            return base._IsCasting() && status == SpellStatus.Casting;
        }

        private void SendSpellStartClientInteraction()
        {
            // Shoule we actually emit client interaction events to everyone? - Logs suggest that we only see this packet firing when the client interacts with -something- and is likely only sent to them
            if (caster is IPlayer player)
            {
                player.Session.EnqueueMessageEncrypted(new ServerSpellStartClientInteraction
                {
                    ClientUniqueId = Parameters.ClientSideInteraction.ClientUniqueId,
                    CastingId      = CastingId,
                    CasterId       = GetPrimaryTargetId()
                });
            }
        }

        /// <summary>
        /// Used when a <see cref="CSI.ClientSideInteraction"/> succeeds
        /// </summary>
        public void SucceedClientInteraction()
        {
            Execute();

            if (Parameters.SpellInfo.Effects.FirstOrDefault(x => (SpellEffectType)x.EffectType == SpellEffectType.Activate) == null)
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
            return Parameters.ClientSideInteraction.Entry != null ? caster.Guid : Parameters.PrimaryTargetId;
        }
    }
}
