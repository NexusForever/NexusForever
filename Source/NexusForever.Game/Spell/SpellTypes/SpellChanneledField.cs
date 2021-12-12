using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Static.Spell;
using NLog;

namespace NexusForever.Game.Spell
{
    [SpellType(CastMethod.ChanneledField)]
    public partial class SpellChanneledField : Spell, ISpellType
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private static CastMethod castMethod = CastMethod.ChanneledField;

        public SpellChanneledField(IUnitEntity caster, ISpellParameters parameters)
            : base(caster, parameters, castMethod)
        {
        }

        public override bool Cast()
        {
            return base.Cast();
        }
    }
}
