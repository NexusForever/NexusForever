using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Systemd;
using Microsoft.Extensions.Hosting.WindowsServices;
using Microsoft.Extensions.Logging;
using NexusForever.API.Character.Client;
using NexusForever.API.Configuration.Model;
using NexusForever.Database.Chat;
using NexusForever.Database.Configuration.Model;
using NexusForever.GameTable;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Configuration;
using NexusForever.Server.ChatServer.Character;
using NexusForever.Server.ChatServer.Chat;
using NexusForever.Server.ChatServer.Job;
using NexusForever.Server.ChatServer.Network.Internal.Handler;
using NLog.Extensions.Logging;
using Quartz;

namespace NexusForever.Server.ChatServer
{
    internal static class ChatServer
    {
        #if DEBUG
        private const string Title = "NexusForever: Chat Server (DEBUG)";
        #else
        private const string Title = "NexusForever: Chat Server (RELEASE)";
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
                        .AddJsonFile("ChatServer.json", false)
                        .AddEnvironmentVariables();
                })
                .ConfigureServices((hb, sc) =>
                {
                    sc.AddHostedService<HostedService>();
                    sc.AddHostedService<NetworkInternalHandlerHostedService>();

                    sc.AddGameTable(
                        hb.Configuration.GetSection("GameTable"));

                    sc.AddChatDatabase(
                        hb.Configuration.GetSection("Database:Chat")
                        .Get<DatabaseConnectionString>());

                    sc.AddCharacterAPIClient(
                        hb.Configuration.GetSection("API:Character")
                        .Get<APIConfig>());

                    sc.AddNetworkInternal();
                    sc.AddScoped<OutboxMessagePublisher>();
                    sc.AddSingleton<OutboxUrgentSignal>();
                    sc.AddHostedService<OutboxUrgentHostedService>();

                    sc.AddNetworkInternalBroker(
                        hb.Configuration.GetSection("Network:Internal")
                        .Get<BrokerConfig>());
                    sc.AddNetworkInternalHandlers();

                    sc.AddChat();
                    sc.AddCharacter();

                    sc.AddQuartz(c =>
                    {
                        c.ScheduleJob<OutboxScheduledJob>(t => t
                            .StartNow()
                            .WithSimpleSchedule(s => s
                                .WithInterval(TimeSpan.FromSeconds(1))
                                .RepeatForever()));
                    });
                    sc.AddQuartzHostedService();
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
