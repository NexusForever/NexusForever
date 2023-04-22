using Microsoft.AspNetCore.Hosting;

namespace NexusForever.WorldServer
{
    public static class WorldServerEmbeddedWebServer
    {
        public static IWebHostBuilder Build(IWebHostBuilder builder)
        {
            return builder.UseStartup<WorldServerStartup>()
                .UseUrls("http://localhost:5000")
                .PreferHostingUrls(false); // Can override in XXX.json
        }
    }
}
