using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NexusForever.Shared;
using NexusForever.Shared.Configuration;
using NLog.Web;

namespace NexusForever.WorldServer
{
    public class WorldServerEmbeddedWebServer : Singleton<WorldServerEmbeddedWebServer>, IShutdownAble
    {
        private IWebHost webHost;

        public WorldServerEmbeddedWebServer Initialise()
        {
            IWebHostBuilder builder = Initialise(SharedConfiguration.Configuration);
            webHost = builder.Build();
            webHost.Start();
            return Instance;
        }

        private static IWebHostBuilder Initialise(IConfiguration configuration) => WebHost.CreateDefaultBuilder()
            .UseConfiguration(configuration)
            .UseStartup<WorldServerStartup>()
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.SetMinimumLevel(LogLevel.Trace);
            })
            .UseNLog()
            .UseUrls($"http://localhost:5000")
            .PreferHostingUrls(false); // Can override in XXX.json

        public void Shutdown()
        {
            using (webHost)
            {
                webHost?.StopAsync()?.Wait();
            }
        }
    }
}
