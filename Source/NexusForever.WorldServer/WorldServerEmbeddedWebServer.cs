using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using NexusForever.Shared.Configuration;
using NLog.Web;

namespace NexusForever.WorldServer
{
    public static class WorldServerEmbeddedWebServer
    {
        public static IWebHostBuilder Build(IWebHostBuilder builder)
        {
            builder.UseConfiguration(SharedConfiguration.Configuration)
                .UseStartup<WorldServerStartup>()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                .UseNLog()
                .UseUrls("http://localhost:5000")
                .PreferHostingUrls(false); // Can override in XXX.json

            return builder;
        }
    }
}
