using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Spell.Event;
using NexusForever.Game.Static.Spell;
using NLog;

namespace NexusForever.Game.Spell
{
    [SpellType(CastMethod.RapidTap)]
    public partial class SpellRapidTap : SpellThreshold, ISpellType
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private static CastMethod castMethod = CastMethod.RapidTap;

        public SpellRapidTap(IUnitEntity caster, ISpellParameters parameters) 
            : base(caster, parameters, castMethod)
        {
        }

        public override bool Cast()
        {
            if (status == SpellStatus.Waiting)
                return base.Cast();

            if (!base.Cast())
                return false;

            if (Parameters.ParentSpellInfo == null)
                events.EnqueueEvent(new SpellEvent(Parameters.SpellInfo.Entry.CastTime / 1000d + Parameters.SpellInfo.Entry.ThresholdTime / 1000d, Finish)); // enqueue spell to be executed after cast time

            events.EnqueueEvent(new SpellEvent(Parameters.SpellInfo.Entry.CastTime / 1000d, Execute)); // enqueue spell to be executed after cast time

            status = SpellStatus.Casting;
            log.Trace($"Spell {Parameters.SpellInfo.Entry.Id} has started casting.");
            return true;
        }

        protected override bool _IsCasting()
        {
            return base._IsCasting() && status == SpellStatus.Casting;
        }
    }
}
