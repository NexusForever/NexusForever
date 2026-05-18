using NexusForever.Game.Abstract.Spell.Event;

namespace NexusForever.Game.Spell.Event
{
    public class SpellEvent : ISpellEvent
    {
        public double Delay { get; private set; }
        public Action Callback { get; }

        /// <summary>
        /// Create a new <see cref="ISpellEvent"/> to be executed after delay in seconds.
        /// </summary>
        public SpellEvent(double delay, Action callback)
        {
            Delay    = delay;
            Callback = callback;
        }

        public void Update(double lastTick)
        {
            Delay -= lastTick;
        }
    }
}
