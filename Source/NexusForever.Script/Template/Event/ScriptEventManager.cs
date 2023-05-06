using Microsoft.Extensions.Logging;
using NexusForever.Shared.Game;

namespace NexusForever.Script.Template.Event
{
    public class ScriptEventManager : IScriptEventManager
    {
        /// <summary>
        /// Invoked when a <see cref="IScriptEvent"/> is invoked, the id of the event is returned if it has one.
        /// </summary>
        public event Action<uint?> OnScriptEvent;

        private readonly List<IPendingScriptEvent> events = new();

        #region Dependency Injection

        private readonly ILogger log;

        public ScriptEventManager(
            ILogger<IScriptEventManager> log)
        {
            this.log = log;
        }

        #endregion

        /// <summary>
        /// Enqueue a new <see cref="IScriptEvent"/> which will be invoked after <see cref="TimeSpan"/>.
        /// </summary>
        public void EnqueueEvent<T>(TimeSpan time, T @event, uint? id) where T : IScriptEvent
        {
            events.Add(new PendingScriptEvent
            {
                Timer = new UpdateTimer(time),
                Event = @event,
                Id    = id
            });

            log.LogTrace("Enqueued event {Name}, {time}, {id}.", typeof(T).Name, time, id);
        }

        /// <summary>
        /// Enqueue a new <see cref="IScriptEvent"/> which will be invoked randomly between 2 <see cref="TimeSpan"/>.
        /// </summary>
        public void EnqueueEvent<T>(TimeSpan start, TimeSpan end, T @event, uint? id) where T : IScriptEvent
        {
            if (end < start)
                throw new ArgumentOutOfRangeException();

            TimeSpan time = TimeSpan.FromSeconds(Random.Shared.Next(start.Seconds, end.Seconds));
            EnqueueEvent(time, @event, id);
        }

        /// <summary>
        /// Cancel <see cref="IScriptEvent"/> with supplied id.
        /// </summary>
        public void CancelEvent(uint id)
        {
            events.RemoveAll(e => e.Id == id);
            log.LogTrace("Cancelled event {id}!", id);
        }

        /// <summary>
        /// Cancel all <see cref="IScriptEvent"/> of type <typeparamref name="T"/>.
        /// </summary>
        public void CancelEvents<T>()
        {
            events.RemoveAll(e => e.Event is T);
            log.LogTrace("Cancelled all {Name} events!", typeof(T).Name);
        }

        /// <summary>
        /// Cancel all <see cref="IScriptEvent"/>.
        /// </summary>
        public void CancelEvents()
        {
            events.Clear();
            log.LogTrace("Cancelled all events.");
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public void Update(double lastTick)
        {
            foreach (IPendingScriptEvent @event in events.ToList())
            {
                @event.Timer.Update(lastTick);
                if (!@event.Timer.HasElapsed)
                    continue;

                @event.Event.Invoke();
                OnScriptEvent?.Invoke(@event.Id);

                events.Remove(@event);
            }
        }
    }
}
