using NexusForever.Database.Configuration;
using NexusForever.Shared.Configuration;

namespace NexusForever.StsServer
{
    public class StsServerConfiguration
    {
        public NetworkConfig Network { get; set; }
        public DatabaseConfig Database { get; set; }
    }
}
