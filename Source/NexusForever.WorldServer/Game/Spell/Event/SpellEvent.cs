using System;
using NexusForever.Shared;

namespace NexusForever.WorldServer.Game.Spell.Event
{
    public class SpellEvent : IUpdate
    {
        public double Delay { get; private set; }
        public Action Callback { get; }

        /// <summary>
        /// Create a new <see cref="SpellEvent"/> to be executed after delay in seconds.
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
