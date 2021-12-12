using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Spell.Event;
using NexusForever.Game.Static.Spell;
using NexusForever.Network.World.Message.Static;
using NLog;

namespace NexusForever.Game.Spell
{
    [SpellType(CastMethod.Channeled)]
    public partial class SpellChanneled : Spell, ISpellType
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private static CastMethod castMethod = CastMethod.Channeled;

        public SpellChanneled(IUnitEntity caster, ISpellParameters parameters)
            : base(caster, parameters, castMethod)
        {
        }

        public override bool Cast()
        {
            if (!base.Cast())
                return false;

            events.EnqueueEvent(new SpellEvent(Parameters.SpellInfo.Entry.ChannelInitialDelay / 1000d, () =>
            {
                CastResult checkResources = CheckResourceConditions();
                if (checkResources != CastResult.Ok)
                {
                    CancelCast(checkResources);
                    return;
                }

                Execute();

                targets.ForEach(t => t.Effects.Clear());
            })); // Execute after initial delay
            events.EnqueueEvent(new SpellEvent(Parameters.SpellInfo.Entry.ChannelMaxTime / 1000d, Finish)); // End Spell Cast

            uint numberOfPulses = (uint)MathF.Floor(Parameters.SpellInfo.Entry.ChannelMaxTime / Parameters.SpellInfo.Entry.ChannelPulseTime); // Calculate number of "ticks" in this spell cast

            // Add ticks at each pulse
            for (int i = 1; i <= numberOfPulses; i++)
                events.EnqueueEvent(new SpellEvent((Parameters.SpellInfo.Entry.ChannelInitialDelay + (Parameters.SpellInfo.Entry.ChannelPulseTime * i)) / 1000d, () =>
                {
                    CastResult checkResources = CheckResourceConditions();
                    if (checkResources != CastResult.Ok)
                    {
                        CancelCast(checkResources);
                        return;
                    }

                    effectTriggerCount.Clear();
                    Execute();

                    targets.ForEach(t => t.Effects.Clear());
                }));

            status = SpellStatus.Casting;
            log.Trace($"Spell {Parameters.SpellInfo.Entry.Id} has started casting.");
            return true;
        }

        protected override bool _IsCasting()
        {
            return base._IsCasting() && (status == SpellStatus.Casting || status == SpellStatus.Executing);
        }
    }
}
