using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NexusForever.Shared.Configuration;
using NLog.Web;

namespace NexusForever.WorldServer
{
    public class WorldServerEmbeddedWebServer
    {
        public static IDisposable Initialise()
        {
            IWebHostBuilder builder = Initialise(SharedConfiguration.Configuration);
            IWebHost webHost = builder.Build();
            webHost.Start();
            return webHost;
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
    }
}