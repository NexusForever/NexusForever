using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NexusForever.WorldServer.Command.Handler;
using NLog.Web;

namespace NexusForever.WorldServer
{
    public class WorldServerEmbeddedWebServer
    {
        public static IWebHostBuilder Initialize(IConfiguration configuration) => WebHost.CreateDefaultBuilder()
            .UseConfiguration(configuration)
            .UseStartup<WorldServerStartup>()
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.SetMinimumLevel(LogLevel.Trace);
            })
            .UseNLog()
            .UseUrls($"http://localhost:5000")
            .PreferHostingUrls(false) // Can override in XXX.json
            .ConfigureServices(services =>
            {
                services.AddSingleton<ICommandHandler, MountCommand>()
                    .AddSingleton<ICommandHandler, AccountCommandHandler>()
                    .AddSingleton<ICommandHandler, TeleportHandler>();
            });
    }
}