using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using NexusForever.Shared;
using NexusForever.Shared.Configuration;
using NexusForever.Shared.Database;
using NexusForever.Shared.Network;
using NexusForever.StsServer.Network;
using NexusForever.StsServer.Network.Message;
using NLog;

namespace NexusForever.StsServer
{
    public class HostedService : IHostedService
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Start <see cref="StsServer"/> and any related resources.
        /// </summary>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            log.Info("Starting...");

            ConfigurationManager<StsServerConfiguration>.Instance.Initialise("StsServer.json");
            DatabaseManager.Instance.Initialise(ConfigurationManager<StsServerConfiguration>.Instance.Config.Database);

            // initialise world after all assets have loaded but before any network handlers might be invoked
            WorldManager.Instance.Initialise(lastTick =>
            {
                NetworkManager<StsSession>.Instance.Update(lastTick);
            });

            // initialise network manager last to make sure the rest of the server is ready for invoked handlers
            MessageManager.Instance.Initialise();
            NetworkManager<StsSession>.Instance.Initialise(ConfigurationManager<StsServerConfiguration>.Instance.Config.Network);

            log.Info("Started!");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Start <see cref="StsServer"/> and any related resources.
        /// </summary>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            log.Info("Stopping...");

            // stop network manager listening for incoming connections
            // it is still possible for incoming packets to be parsed though won't be handled once the world thread is stopped
            NetworkManager<StsSession>.Instance.Shutdown();

            // stop world manager processing the world thread
            // at this point no incoming packets will be handled
            WorldManager.Instance.Shutdown();

            log.Info("Stopped!");
            return Task.CompletedTask;
        }
    }
}
