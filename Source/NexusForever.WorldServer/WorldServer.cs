using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Systemd;
using Microsoft.Extensions.Hosting.WindowsServices;
using NexusForever.Database;
using NexusForever.Game;
using NexusForever.Game.Configuration.Model;
using NexusForever.GameTable;
using NexusForever.Network.Configuration.Model;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Configuration;
using NexusForever.Script;
using NexusForever.Script.Configuration.Model;
using NexusForever.Shared;
using NexusForever.Shared.Configuration;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Internal.Handler;
using NexusForever.WorldServer.Service;
using NLog;
using NLog.Extensions.Logging;

namespace NexusForever.WorldServer
{
    internal static class WorldServer
    {
        #if DEBUG
        private const string Title = "NexusForever: World Server (DEBUG)";
        #else
        private const string Title = "NexusForever: World Server (RELEASE)";
        #endif

        private static readonly NLog.ILogger log = LogManager.GetCurrentClassLogger();

        private static readonly CancellationTokenSource cancellationToken = new();

        private static async Task Main()
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));

            IHostBuilder builder = new HostBuilder()
                .ConfigureLogging(lb =>
                {
                    lb.AddNLog();
                })
                .ConfigureAppConfiguration(cb =>
                {
                    cb.AddJsonFile("WorldServer.json", false)
                        .AddEnvironmentVariables();
                })
                .ConfigureServices((hb, sc) =>
                {
                    // register world server service first since it needs to execute before the web host
                    sc.AddHostedService<HostedService>();
                    sc.AddHostedService<NetworkInternalHandlerHostedService>();
                    sc.AddHostedService<OnlineHostedService>();

                    sc.AddOptions<NetworkConfig>()
                        .Bind(hb.Configuration.GetSection("Network"));
                    sc.AddOptions<RealmConfig>()
                        .Bind(hb.Configuration.GetSection("Realm"));
                    sc.AddOptions<ScriptConfig>()
                        .Bind(hb.Configuration.GetSection("Script"));

                    sc.AddNetworkInternal();
                    sc.AddNetworkInternalBroker(hb.Configuration.GetSection("Network:Internal").Get<BrokerConfig>());
                    sc.AddNetworkInternalHandlers();

                    sc.AddSingletonLegacy<ISharedConfiguration, SharedConfiguration>();
                    sc.AddDatabase();
                    sc.AddGame();
                    sc.AddGameTable(
                        hb.Configuration.GetSection("GameTable"));
                    sc.AddWorldNetwork();
                    sc.AddScript();
                    sc.AddShared();
                    sc.AddWorld();
                })
                .ConfigureWebHostDefaults(wb =>
                {
                    WorldServerEmbeddedWebServer.Build(wb);
                })
                .UseWindowsService()
                .UseSystemd();

            if (!WindowsServiceHelpers.IsWindowsService() && !SystemdHelpers.IsSystemdService())
                Console.Title = Title;

            try
            {
                var host = builder.Build();
                await host.RunAsync(cancellationToken.Token);
            }
            catch (Exception e)
            {
                log.Fatal(e);
            }
        }

        /// <summary>
        /// Request shutdown of <see cref="WorldServer"/>.
        /// </summary>
        public static void Shutdown()
        {
            cancellationToken.Cancel();
        }
    }
}
