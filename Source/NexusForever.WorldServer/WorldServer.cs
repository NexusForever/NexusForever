using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Systemd;
using Microsoft.Extensions.Hosting.WindowsServices;
using NexusForever.Shared.Configuration;
using NLog;

namespace NexusForever.WorldServer
{
    internal static class WorldServer
    {
        #if DEBUG
        private const string Title = "NexusForever: World Server (DEBUG)";
        #else
        private const string Title = "NexusForever: World Server (RELEASE)";
        #endif

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private static readonly CancellationTokenSource cancellationToken = new();

        private static async Task Main()
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));

            SharedConfiguration.Instance.Initialise<WorldServerConfiguration>("WorldServer.json");

            IHostBuilder builder = new HostBuilder() //Host.CreateDefaultBuilder()
                // register world server service first since it needs to execute before the web host
                .ConfigureServices(sc =>
                {
                    sc.AddHostedService<HostedService>();
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
