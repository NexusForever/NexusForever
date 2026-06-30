using System.Reflection;
using System.Threading.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Systemd;
using Microsoft.Extensions.Hosting.WindowsServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NexusForever.API.Character.Client;
using NexusForever.API.Configuration.Model;
using NexusForever.Database.Configuration.Model;
using NexusForever.Database.Query;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Configuration;
using NexusForever.Server.Character.Configuration;
using NexusForever.Server.Character.Game;
using NexusForever.Server.Character.Network.Internal.Handler;
using NLog.Extensions.Logging;

namespace NexusForever.Server.Character
{
    internal static class CharacterServer
    {
        #if DEBUG
        private const string Title = "NexusForever: Character Server (DEBUG)";
        #else
        private const string Title = "NexusForever: Character Server (RELEASE)";
        #endif

        internal static async Task Main(string[] args)
        {
            string basePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            Directory.SetCurrentDirectory(basePath);

            var builder = new HostBuilder()
                .ConfigureLogging(c =>
                {
                    c.ClearProviders()
                        .SetMinimumLevel(LogLevel.Trace)
                        .AddNLog();
                })
                .ConfigureAppConfiguration(cb =>
                {
                    cb.SetBasePath(basePath)
                        .AddJsonFile("CharacterServer.json", false)
                        .AddEnvironmentVariables();
                })
                .ConfigureServices((hb, sc) =>
                {
                    sc.AddHostedService<NetworkInternalHandlerHostedService>();

                    sc.AddOptions<WhoOptions>()
                        .Bind(hb.Configuration.GetSection("Who"));

                    sc.AddSingleton(sp =>
                    {
                        var whoOptions = sp.GetRequiredService<IOptions<WhoOptions>>().Value;

                        return PartitionedRateLimiter.Create<Identity, Identity>(c =>
                        {
                            return RateLimitPartition.GetFixedWindowLimiter(c, args =>
                            {
                                return new FixedWindowRateLimiterOptions
                                {
                                    PermitLimit       = (int)whoOptions.PermitLimit,
                                    Window            = TimeSpan.FromSeconds(whoOptions.WindowSeconds),
                                    AutoReplenishment = true
                                };
                            });
                        });
                    });

                    sc.AddQueryDatabase(
                        hb.Configuration.GetSection("Database:Query")
                        .Get<DatabaseConnectionString>());

                    sc.AddCharacterAPIClient(
                        hb.Configuration.GetSection("API:Character")
                        .Get<APIConfig>());

                    sc.AddNetworkInternalBroker(
                        hb.Configuration.GetSection("Network:Internal")
                        .Get<BrokerConfig>());

                    sc.AddNetworkInternalHandlers();

                    sc.AddGame();
                })
                .UseWindowsService()
                .UseSystemd();

            if (!WindowsServiceHelpers.IsWindowsService() && !SystemdHelpers.IsSystemdService())
                Console.Title = Title;

            IHost host = builder.Build();
            await host.RunAsync();
        }
    }
}
