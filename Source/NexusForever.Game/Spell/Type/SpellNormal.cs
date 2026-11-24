using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Abstract.Spell.Target;
using NexusForever.Game.Abstract.Spell.Validator;
using NexusForever.Game.Spell.Event;
using NexusForever.Game.Static.Spell;

namespace NexusForever.Game.Spell.Type
{
    public class SpellNormal : Spell
    {
        public override CastMethod CastMethod => CastMethod.Normal;

        #region Dependency Injection

        private readonly ILogger log;

        public SpellNormal(
            ILogger<SpellNormal> log,
            ISpellTargetInfoCollection spellTargetInfoCollection,
            IGlobalSpellManager globalSpellManager,
            ICastResultValidatorManager castResultValidatorManager,
            IDisableManager disableManager)
            : this((ILogger)log, spellTargetInfoCollection, globalSpellManager, castResultValidatorManager, disableManager)
        {
        }

        /// <summary>
        /// Constructor for derived cast methods that share the normal cast timeline, so that they keep
        /// their own logger category. Not public, so dependency injection always picks the constructor
        /// above when resolving <see cref="SpellNormal"/> itself.
        /// </summary>
        protected SpellNormal(
            ILogger log,
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

            uint castTime = Parameters.CastTimeOverride > -1 ? (uint)Parameters.CastTimeOverride : Parameters.SpellInfo.Entry.CastTime;
            events.EnqueueEvent(new SpellEvent(castTime / 1000d, () => { Execute(); })); // enqueue spell to be executed after cast time

            status = SpellStatus.Casting;
            log.LogTrace($"Spell {Parameters.SpellInfo.Entry.Id} has started casting.");
            return true;
        }

        protected override bool _IsCasting()
        {
            return base._IsCasting() && status == SpellStatus.Casting;
        }
    }
}
