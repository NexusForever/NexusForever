namespace NexusForever.Game.Abstract.Event
{
    public interface IPublicEventFactory
    {
        /// <summary>
        /// Create a new <see cref="IPublicEvent"/> with the supplied id.
        /// </summary>
        IPublicEvent CreateEvent(uint id);
    }
}
