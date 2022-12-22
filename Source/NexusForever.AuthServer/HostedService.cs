using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using NexusForever.AuthServer.Network;
using NexusForever.Database;
using NexusForever.Database.Configuration.Model;
using NexusForever.Game.Server;
using NexusForever.Network;
using NexusForever.Network.Configuration.Model;
using NexusForever.Network.Message;
using NexusForever.Shared;
using NexusForever.Shared.Configuration;
using NLog;

namespace NexusForever.AuthServer
{
    public class HostedService : IHostedService
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Start <see cref="AuthServer"/> and any related resources.
        /// </summary>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            log.Info("Starting...");

            DatabaseManager.Instance.Initialise(SharedConfiguration.Instance.Get<DatabaseConfig>());

            ServerManager.Instance.Initialise();

            // initialise world after all assets have loaded but before any network or command handlers might be invoked
            WorldManager.Instance.Initialise(lastTick =>
            {
                NetworkManager<AuthSession>.Instance.Update(lastTick);
            });

            // initialise network and command managers last to make sure the rest of the server is ready for invoked handlers
            MessageManager.Instance.Initialise();
            NetworkManager<AuthSession>.Instance.Initialise(SharedConfiguration.Instance.Get<NetworkConfig>());

            log.Info("Started!");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Shutdown <see cref="AuthServer"/> and any related resources.
        /// </summary>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            log.Info("Stopping...");

            // stop network manager listening for incoming connections
            // it is still possible for incoming packets to be parsed though won't be handled once the world thread is stopped
            NetworkManager<AuthSession>.Instance.Shutdown();

            // stop server manager pinging other servers
            ServerManager.Instance.Shutdown();

            // stop world manager processing the world thread
            // at this point no incoming packets will be handled
            WorldManager.Instance.Shutdown();

            log.Info("Stopped!");
            return Task.CompletedTask;
        }
    }
}
