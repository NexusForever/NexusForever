using Microsoft.Extensions.Configuration;

namespace NexusForever.Shared.Configuration
{
    public sealed class ConfigurationManager<T> : AbstractManager<ConfigurationManager<T>>
    {
        public T Config { get; private set; }

        private ConfigurationManager()
        {
        }

        public ConfigurationManager<T> Initialise(string file)
        {
            SharedConfiguration.Initialise(file);
            Config = SharedConfiguration.Configuration.Get<T>();
            return Instance;
        }
    }
}
