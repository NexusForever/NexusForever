using Microsoft.Extensions.Hosting;
using NexusForever.Network.Internal.Message.Player;
using NexusForever.Network.Internal.Message.Who;
using Rebus.Bus;

namespace NexusForever.Server.Character.Network.Internal.Handler
{
    public class NetworkInternalHandlerHostedService : IHostedService
    {
        #region Dependency Injection

        private readonly IBus _bus;

        public NetworkInternalHandlerHostedService(
            IBus bus)
        {
            _bus = bus;
        }

        #endregion

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _bus.Subscribe<PlayerGuildAssociationUpdatedMessage>();
            await _bus.Subscribe<PlayerInfoRequestMessage>();
            await _bus.Subscribe<PlayerLoggedInMessage>();
            await _bus.Subscribe<PlayerLoggedOutMessage>();
            await _bus.Subscribe<PlayerStatUpdatedMessage>();
            await _bus.Subscribe<PlayerWorldZoneUpdatedMessage>();

            await _bus.Subscribe<WhoRequestMessage>();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
