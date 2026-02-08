using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Systemd;
using Microsoft.Extensions.Hosting.WindowsServices;
using Microsoft.Extensions.Logging;
using NexusForever.API.Character.Client;
using NexusForever.API.Configuration.Model;
using NexusForever.Database.Configuration.Model;
using NexusForever.Database.Group;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Configuration;
using NexusForever.Server.GroupServer.Character;
using NexusForever.Server.GroupServer.Group;
using NexusForever.Server.GroupServer.Job;
using NexusForever.Server.GroupServer.Network.Internal;
using NLog.Extensions.Logging;
using Quartz;

namespace NexusForever.Server.GroupServer
{
    internal static class GroupServer
    {
        #if DEBUG
        private const string Title = "NexusForever: Group Server (DEBUG)";
        #else
        private const string Title = "NexusForever: Group Server (RELEASE)";
        #endif

        internal static async Task Main()
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
                        .AddJsonFile("GroupServer.json", false)
                        .AddEnvironmentVariables();
                })
                .ConfigureServices((hb, sc) =>
                {
                    sc.AddHostedService<HostedService>();

                    sc.AddGroupDatabase(
                        hb.Configuration.GetSection("Database:Group")
                        .Get<DatabaseConnectionString>());

                    sc.AddCharacterAPIClient(
                        hb.Configuration.GetSection("API:Character")
                        .Get<APIConfig>());

                    sc.AddNetworkInternal();
                    sc.AddTransient<OutboxMessagePublisher>();

                    sc.AddNetworkInternalBroker(
                        hb.Configuration.GetSection("Network:Internal")
                        .Get<BrokerConfig>());
                    sc.AddNetworkInternalHandlers();

                    sc.AddGroup();
                    sc.AddCharacter();
                    sc.AddScheduledJobs();
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
