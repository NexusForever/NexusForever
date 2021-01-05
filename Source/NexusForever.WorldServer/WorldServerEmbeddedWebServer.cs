using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NexusForever.Shared;
using NexusForever.Shared.Configuration;
using NLog.Web;

namespace NexusForever.WorldServer
{
    public class WorldServerEmbeddedWebServer : AbstractManager<WorldServerEmbeddedWebServer>
    {
        private IWebHost webHost;

        public override WorldServerEmbeddedWebServer Initialise()
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

        public override void Shutdown()
        {
            using (webHost)
            {
                webHost?.StopAsync()?.Wait();
            }
        }
    }
}
