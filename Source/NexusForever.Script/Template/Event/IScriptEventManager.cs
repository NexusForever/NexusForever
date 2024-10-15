using NexusForever.Shared;

namespace NexusForever.Script.Template.Event
{
    public interface IScriptEventManager : IUpdate
    {
        /// <summary>
        /// Invoked when a <see cref="IScriptEvent"/> is invoked, the id of the event is returned if it has one.
        /// </summary>
        event Action<IScriptEvent, uint?> OnScriptEvent;

        /// <summary>
        /// Enqueue a new <see cref="IScriptEvent"/> which will be invoked after <see cref="TimeSpan"/>.
        /// </summary>
        void EnqueueEvent<T>(TimeSpan time, T @event, uint? id = null) where T : IScriptEvent;

        /// <summary>
        /// Enqueue a new <see cref="IScriptEvent"/> which will be invoked randomly between 2 <see cref="TimeSpan"/>.
        /// </summary>
        void EnqueueEvent<T>(TimeSpan start, TimeSpan end, T @event, uint? id = null) where T : IScriptEvent;

        /// <summary>
        /// Cancel <see cref="IScriptEvent"/> with supplied id.
        /// </summary>
        void CancelEvent(uint id);

        /// <summary>
        /// Cancel all <see cref="IScriptEvent"/> of type <typeparamref name="T"/>.
        /// </summary>
        void CancelEvents<T>();

        /// <summary>
        /// Cancel all <see cref="IScriptEvent"/>.
        /// </summary>
        void CancelEvents();
    }
}
