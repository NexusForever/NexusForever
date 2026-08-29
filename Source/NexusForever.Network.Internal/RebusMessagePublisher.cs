using Rebus.Bus;

namespace NexusForever.Network.Internal
{
    public class RebusMessagePublisher : IInternalMessagePublisher
    {
        #region Dependency Injection

        private readonly IBus _bus;

        public RebusMessagePublisher(
            IBus bus)
        {
            _bus = bus;
        }

        #endregion

        /// <summary>
        /// Publish a message to the internal message broker via Rebus.
        /// </summary>
        /// <param name="message">Message to publish.</param>
        public async Task PublishAsync(object message)
        {
            await _bus.Publish(message);
        }
    }
}
