namespace NexusForever.Game.Abstract.PublicEvent
{
    public interface IPublicEventFactory
    {
        /// <summary>
        /// Create a new <see cref="IPublicEvent"/> with the supplied id.
        /// </summary>
        IPublicEvent CreateEvent(uint id);
    }
}
