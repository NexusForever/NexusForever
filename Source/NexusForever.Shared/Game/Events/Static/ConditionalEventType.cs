namespace NexusForever.Shared.Game.Events.Static
{
    public enum ConditionalEventType
    {
        /// <summary>
        /// <see cref="IEvent"/> will be moved to the back of the processing queue if <see cref="IEvent.CanExecute"/> returns false.
        /// </summary>
        Standard,

        /// <summary>
        /// <see cref="IEvent"/> will block all events until <see cref="IEvent.CanExecute"/> returns true.
        /// </summary>
        Blocking
    }
}
