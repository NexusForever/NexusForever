
namespace NexusForever.Network.Internal
{
    public interface IInternalMessagePublisher
    {
        /// <summary>
        /// Publish a message to the internal message broker.
        /// </summary>
        /// <param name="message">Message to publish.</param>
        Task PublishAsync(object message);
    }
}
