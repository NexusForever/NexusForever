using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Systemd;
using Microsoft.Extensions.Hosting.WindowsServices;
using Microsoft.Extensions.Logging;
using NexusForever.API.Account.Client;
using NexusForever.API.Character.Client;
using NexusForever.API.Configuration.Model;
using NexusForever.Database.Configuration.Model;
using NexusForever.Database.Friendship;
using NexusForever.GameTable;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Configuration;
using NexusForever.Server.Friendship.Configuration;
using NexusForever.Server.Friendship.Game.Account;
using NexusForever.Server.Friendship.Game.Character;
using NexusForever.Server.Friendship.Game.Friend;
using NexusForever.Server.Friendship.Job;
using NexusForever.Server.Friendship.Network.Internal;
using NexusForever.Server.Friendship.Network.Internal.Handler;
using NexusForever.Shared.Configuration;
using NLog.Extensions.Logging;
using Quartz;

namespace NexusForever.Server.Friendship
{
    internal static class FriendshipServer
    {
        #if DEBUG
        private const string Title = "NexusForever: Friendship Server (DEBUG)";
        #else
        private const string Title = "NexusForever: Friendship Server (RELEASE)";
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
                        .AddNexusForeverJson("FriendshipServer.json")
                        .AddEnvironmentVariables();
                })
                .ConfigureServices((hb, sc) =>
                {
                    sc.AddHostedService<HostedService>();
                    sc.AddHostedService<NetworkInternalHandlerHostedService>();

                    sc.AddOptions<LimitOptions>()
                        .Bind(hb.Configuration.GetSection("Limit"));

                    sc.AddOptions<InviteOptions>()
                        .Bind(hb.Configuration.GetSection("Invite"));

                    sc.AddGameTable(
                        hb.Configuration.GetSection("GameTable"));

                    sc.AddFriendshipDatabase(
                        hb.Configuration.GetSection("Database:Friendship")
                        .Get<DatabaseConnectionString>());

                    sc.AddAccountAPIClient(
                        hb.Configuration.GetSection("API:Account")
                        .Get<APIConfig>());

                    sc.AddCharacterAPIClient(
                        hb.Configuration.GetSection("API:Character")
                        .Get<APIConfig>());

                    sc.AddNetworkInternalBroker(
                        hb.Configuration.GetSection("Network:Internal")
                        .Get<BrokerConfig>());
                    sc.AddNetworkInternalHandlers();
                    sc.AddTransient<OutboxMessagePublisher>();

                    sc.AddAccount();
                    sc.AddCharacter();
                    sc.AddFriend();

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
