using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using NexusForever.Game.Abstract;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Server;

namespace NexusForever.WorldServer.Service
{
    public class OnlineHostedService : IHostedService
    {
        #region Dependency Injection

        private readonly IRealmContext realmContext;
        private readonly IInternalMessagePublisher messagePublisher;

        public OnlineHostedService(
            IRealmContext realmContext,
            IInternalMessagePublisher messagePublisher)
        {
            this.realmContext     = realmContext;
            this.messagePublisher = messagePublisher;
        }

        #endregion

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await messagePublisher.PublishAsync(new ServerWorldOnlineMessage
            {
                RealmId = realmContext.RealmId
            });
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await messagePublisher.PublishAsync(new ServerWorldOfflineMessage
            {
                RealmId = realmContext.RealmId
            });
        }
    }
}
