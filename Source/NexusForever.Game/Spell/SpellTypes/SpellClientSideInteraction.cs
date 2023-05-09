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

        /// <summary>
        /// Used when a <see cref="IClientSideInteraction"/> succeeds
        /// </summary>
        /// /// <remarks>
        /// Some spells offer a CSI "Event" in the client - a dialog box, a mini-game, etc. - but, do not have a ClientUniqueId as not triggered by player directly doing something.
        /// In this case they are spells cast by something else that require player interaction, e.g. when you get rooted but can break the root by holding down a key.
        /// We only generated a <see cref="IClientSideInteraction"/> instance in the cases where the client delivers a ClientUniqueId.
        /// </remarks>
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

        protected override uint GetPrimaryTargetId()
        {
            return Parameters.ClientSideInteraction?.Entry != null ? caster.Guid : Parameters.PrimaryTargetId;
        }
    }
}
