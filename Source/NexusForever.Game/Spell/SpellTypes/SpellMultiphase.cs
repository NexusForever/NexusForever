using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Spell.Event;
using NexusForever.Game.Static.Spell;
using NexusForever.GameTable.Model;
using NLog;

namespace NexusForever.Game.Spell
{
    [SpellType(CastMethod.Multiphase)]
    public partial class SpellMultiphase : Spell, ISpellType
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private static CastMethod castMethod = CastMethod.Multiphase;

        public SpellMultiphase(IUnitEntity caster, ISpellParameters parameters) 
            : base(caster, parameters, castMethod)
        {
        }

        public override bool Cast()
        {
            if (!base.Cast())
                return false;

            uint spellDelay = 0;
            for (int i = 0; i < Parameters.SpellInfo.Phases.Count; i++)
            {
                int index = i;
                SpellPhaseEntry spellPhase = Parameters.SpellInfo.Phases[i];
                spellDelay += spellPhase.PhaseDelay;
                events.EnqueueEvent(new SpellEvent(spellDelay / 1000d, () =>
                {
                    currentPhase = (byte)spellPhase.OrderIndex;
                    effectTriggerCount.Clear();
                    Execute();

                    if (i == Parameters.SpellInfo.Phases.Count - 1)
                    {
                        status = SpellStatus.Finishing;
                        log.Trace($"SpellMultiphase {Parameters.SpellInfo.Entry.Id} has finished executing.");
                    }
                }));
            }

            status = SpellStatus.Casting;
            log.Trace($"SpellMultiphase {Parameters.SpellInfo.Entry.Id} has started casting.");
            return true;
        }

        protected override bool _IsCasting()
        {
            return base._IsCasting() && (status == SpellStatus.Casting || status == SpellStatus.Executing);
        }
    }
}
