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
using Microsoft.Extensions.Logging;
using NexusForever.Database;
using NexusForever.Database.Telemetry;
using NexusForever.Game;
using NexusForever.Game.Configuration.Model;
using NexusForever.GameTable;
using NexusForever.Network.Configuration.Model;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Configuration;
using NexusForever.Network.Internal.Telemetry.Trace;
using NexusForever.Network.Telemetry;
using NexusForever.Script;
using NexusForever.Script.Configuration.Model;
using NexusForever.Shared;
using NexusForever.Shared.Configuration;
using NexusForever.Telemetry;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Internal.Handler;
using NexusForever.WorldServer.Service;
using NexusForever.WorldServer.Telemetry.Metric;
using NLog;
using NLog.Extensions.Logging;
using OpenTelemetry;

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
            string basePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            Directory.SetCurrentDirectory(basePath);

            IHostBuilder builder = new HostBuilder()
                .ConfigureLogging((hb, c) =>
                {
                    c.ClearProviders()
                        .AddConfiguration(hb.Configuration.GetSection("Logging"))
                        .AddNLog(new NLogProviderOptions
                        {
                            RemoveLoggerFactoryFilter = false
                        });
                })
                .ConfigureAppConfiguration(cb =>
                {
                    cb.SetBasePath(basePath)
                        .AddJsonFile("WorldServer.json", false)
                        .AddJsonFile("Logging.json", true)
                        .AddEnvironmentVariables();
                })
                .ConfigureServices((hb, sc) =>
                {
                    OpenTelemetryBuilder otb = sc.AddNexusForeverTelemetry(
                        hb.Configuration.GetSection("Telemetry"))?
                            .AddNetworkInternalTracing()
                            .AddNetworkTracing()
                            .AddEntityFrameworkTracing()
                            .AddWorldServerMetrics();

                    // register world server service first since it needs to execute before the web host
                    sc.AddHostedService<ConfigurationHostedService>();
                    sc.AddHostedService<GameTableHostedService>();
                    sc.AddHostedService<SpellHostedService>();
                    sc.AddHostedService<NetworkInternalHandlerHostedService>();
                    sc.AddHostedService<OnlineHostedService>();
                    sc.AddHostedService<HostedService>();
                    sc.AddHostedService<LootHostedService>();

                    sc.AddOptions<NetworkConfig>()
                        .Bind(hb.Configuration.GetSection("Network"));
                    sc.AddOptions<RealmConfig>()
                        .Bind(hb.Configuration.GetSection("Realm"));
                    sc.AddOptions<ScriptConfig>()
                        .Bind(hb.Configuration.GetSection("Script"));

                    sc.AddNetworkInternalBroker(hb.Configuration.GetSection("Network:Internal").Get<BrokerConfig>());
                    sc.AddNetworkInternalHandlers();

                    sc.AddSingletonLegacy<ISharedConfiguration, SharedConfiguration>();
                    sc.AddDatabase();
                    sc.AddGame();
                    sc.AddGameTable(
                        hb.Configuration.GetSection("GameTable"));
                    sc.AddWorldNetwork();
                    sc.AddScript();
                    sc.AddWorld();

                    sc.AddSingleton<IWorldManager, WorldManager>();
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
