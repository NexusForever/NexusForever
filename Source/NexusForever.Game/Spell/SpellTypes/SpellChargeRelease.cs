using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Spell.Event;
using NexusForever.Game.Static.Spell;
using NexusForever.GameTable.Model;
using NLog;

namespace NexusForever.Game.Spell
{
    [SpellType(CastMethod.ChargeRelease)]
    public partial class SpellChargeRelease : SpellThreshold, ISpellType
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private static CastMethod castMethod = CastMethod.ChargeRelease;

        public SpellChargeRelease(IUnitEntity caster, ISpellParameters parameters) 
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
            {
                totalThresholdTimer = (uint)(Parameters.SpellInfo.Entry.ThresholdTime / 1000d);

                // Keep track of cast time increments as we create timers to adjust thresholdValue
                uint nextCastTime = 0;

                // Create timers for each thresholdEntry's timer increment
                foreach (Spell4ThresholdsEntry thresholdsEntry in Parameters.SpellInfo.Thresholds)
                {
                    nextCastTime += thresholdsEntry.ThresholdDuration;

                    if (thresholdsEntry.OrderIndex == 0)
                        continue;

                    events.EnqueueEvent(new SpellEvent(Parameters.SpellInfo.Entry.CastTime / 1000d + nextCastTime / 1000d, () =>
                    {
                        thresholdValue = thresholdsEntry.OrderIndex;
                        SendThresholdUpdate();
                    }));
                }
            }

            events.EnqueueEvent(new SpellEvent(Parameters.SpellInfo.Entry.CastTime / 1000d, Execute)); // enqueue spell to be executed after cast time

            status = SpellStatus.Casting;
            log.Trace($"Spell {Parameters.SpellInfo.Entry.Id} has started casting.");
            return true;
        }

        protected override bool _IsCasting()
        {
            return base._IsCasting() && (status == SpellStatus.Casting || status == SpellStatus.Executing || status == SpellStatus.Waiting); ;
        }
    }
}
