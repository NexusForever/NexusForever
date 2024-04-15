using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Systemd;
using Microsoft.Extensions.Hosting.WindowsServices;
using NexusForever.Database;
using NexusForever.Game;
using NexusForever.GameTable;
using NexusForever.Network;
using NexusForever.Network.World;
using NexusForever.Script;
using NexusForever.Script.Configuration.Model;
using NexusForever.Shared;
using NexusForever.Shared.Configuration;
using NexusForever.WorldServer.Network;
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
                    cb.AddJsonFile("WorldServer.json", false);
                })
                .ConfigureServices((hb, sc) =>
                {
                    // register world server service first since it needs to execute before the web host
                    sc.AddHostedService<HostedService>();

                    sc.AddOptions<ScriptConfig>().Bind(hb.Configuration.GetSection("Script"));

                    sc.AddSingletonLegacy<ISharedConfiguration, SharedConfiguration>();
                    sc.AddDatabase();
                    sc.AddGame();
                    sc.AddGameTable();
                    sc.AddNetwork<WorldSession>();
                    sc.AddNetworkWorld();
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
