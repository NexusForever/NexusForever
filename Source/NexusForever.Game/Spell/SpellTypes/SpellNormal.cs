using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Spell.Event;
using NexusForever.Game.Static.Spell;
using NLog;

namespace NexusForever.Game.Spell
{
    [SpellType(CastMethod.Normal)]
    public partial class SpellNormal : Spell, ISpellType
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private static CastMethod castMethod = CastMethod.Normal;

        public SpellNormal(IUnitEntity caster, ISpellParameters parameters) 
            : base(caster, parameters, castMethod)
        {
        }

        public override bool Cast()
        {
            if (!base.Cast())
                return false;

            uint castTime = Parameters.CastTimeOverride > -1 ? (uint)Parameters.CastTimeOverride : Parameters.SpellInfo.Entry.CastTime;
            events.EnqueueEvent(new SpellEvent(castTime / 1000d, () => { Execute(); })); // enqueue spell to be executed after cast time

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
