using System;
using System.Collections.Generic;
using NexusForever.Shared.Game.Events.Static;
using NLog;

namespace NexusForever.Shared.Game.Events
{
    public class EventQueue : IUpdate
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        
        private class PendingEvent
        {
            public IEvent Event { get; init; }
            public ConditionalEventType Type { get; init; }
        }
        
        /// <summary>
        /// Returns if there are any <see cref="IEvent"/>'s in the queue.
        /// </summary>
        public bool PendingEvents => events.Count != 0;

        private readonly Queue<PendingEvent> events = new();

        /// <summary>
        /// Enqueue <see cref="IEvent"/> to be executed.
        /// </summary>
        public void EnqueueEvent(IEvent @event, ConditionalEventType type = ConditionalEventType.Standard)
        {
            events.Enqueue(new PendingEvent
            {
                Event = @event,
                Type  = type
            });
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public virtual void Update(double lastTick)
        {
            if (!PendingEvents)
                return;

            var newEvents = new List<PendingEvent>();
            while (events.TryPeek(out PendingEvent pending))
            {
                if (!pending.Event.CanExecute())
                {
                    switch (pending.Type)
                    {
                        case ConditionalEventType.Standard:
                            newEvents.Add(events.Dequeue());
                            continue;
                        case ConditionalEventType.Blocking:
                            return;
                        default:
                            throw new NotImplementedException();
                    }
                }

                events.Dequeue();

                try
                {
                    pending.Event.Execute();
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                }
            }

            foreach (PendingEvent @event in newEvents)
                events.Enqueue(@event);
        }
    }
}
