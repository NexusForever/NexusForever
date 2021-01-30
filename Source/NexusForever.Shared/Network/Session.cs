using System;
using System.Collections.Generic;
using NexusForever.Shared.Game.Events;
using NLog;

namespace NexusForever.Shared.Network
{
    public abstract class Session : IUpdate
    {
        protected static readonly ILogger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Returns if there is a pending <see cref="IEvent"/> that has yet to complete.
        /// </summary>
        public bool PendingEvent => events.Count != 0;

        private readonly Queue<IEvent> events = new();

        /// <summary>
        /// Enqueue <see cref="IEvent"/> to be delay executed.
        /// </summary>
        public void EnqueueEvent(IEvent @event)
        {
            events.Enqueue(@event);
        }

        public virtual void Update(double lastTick)
        {
            while (events.TryPeek(out IEvent @event))
            {
                if (!@event.CanExecute())
                    continue;

                events.Dequeue();

                try
                {
                    @event.Execute();
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                }
            }
        }
    }
}
