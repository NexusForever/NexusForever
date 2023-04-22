using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NexusForever.Database;
using NexusForever.Database.Configuration.Model;
using NexusForever.Network;
using NexusForever.Network.Configuration.Model;
using NexusForever.Shared;
using NexusForever.Shared.Configuration;
using NexusForever.StsServer.Network;
using NexusForever.StsServer.Network.Message;

namespace NexusForever.StsServer
{
    public class HostedService : IHostedService
    {
        private readonly ILogger log;
        private readonly IWorldManager worldManager;

        public HostedService(
            IServiceProvider sp,
            ILogger<IHostedService> log,
            IWorldManager worldManager)
        {
            LegacyServiceProvider.Provider = sp;

            this.log          = log;
            this.worldManager = worldManager;
        }

        /// <summary>
        /// Start <see cref="StsServer"/> and any related resources.
        /// </summary>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            log.LogInformation("Starting...");

            SharedConfiguration.Instance.Initialise<StsServerConfiguration>();

            DatabaseManager.Instance.Initialise(SharedConfiguration.Instance.Get<DatabaseConfig>());

            // initialise world after all assets have loaded but before any network handlers might be invoked
            worldManager.Initialise(lastTick =>
            {
                NetworkManager<StsSession>.Instance.Update(lastTick);
            });

            // initialise network manager last to make sure the rest of the server is ready for invoked handlers
            MessageManager.Instance.Initialise();
            NetworkManager<StsSession>.Instance.Initialise(SharedConfiguration.Instance.Get<NetworkConfig>());

            log.LogInformation("Started!");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Start <see cref="StsServer"/> and any related resources.
        /// </summary>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            log.LogInformation("Stopping...");

            // stop network manager listening for incoming connections
            // it is still possible for incoming packets to be parsed though won't be handled once the world thread is stopped
            NetworkManager<StsSession>.Instance.Shutdown();

            // stop world manager processing the world thread
            // at this point no incoming packets will be handled
            worldManager.Shutdown();

            log.LogInformation("Stopped!");
            return Task.CompletedTask;
        }
    }
}
