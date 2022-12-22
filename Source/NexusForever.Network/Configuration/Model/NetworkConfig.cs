using NexusForever.Shared.Configuration;

namespace NexusForever.Network.Configuration.Model
{
    [ConfigurationBind]
    public class NetworkConfig
    {
        public string Host { get; set; }
        public ushort Port { get; set; }
    }
}
