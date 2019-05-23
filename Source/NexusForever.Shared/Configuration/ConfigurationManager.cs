using Microsoft.Extensions.Configuration;

namespace NexusForever.Shared.Configuration
{
    public static class ConfigurationManager<T>
    {
        public static T Config { get; private set; }
        public static IConfiguration GetConfiguration() => SharedConfiguration.Configuration;

        public static void Initialise(string file)
        {
            SharedConfiguration.Initialise(file);
            Config = SharedConfiguration.Configuration.Get<T>();
        }
    }
}
