using NexusForever.Database.Configuration.Model;
using NexusForever.Network.Configuration.Model;

namespace NexusForever.StsServer
{
    public class StsServerConfiguration
    {
        public NetworkConfig Network { get; set; }
        public DatabaseConfig Database { get; set; }
    }
}
