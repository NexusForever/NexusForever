using NexusForever.Database.Configuration.Model;
using NexusForever.Network.Configuration.Model;

namespace NexusForever.AuthServer
{
    public class AuthServerConfiguration
    {
        public NetworkConfig Network { get; set; }
        public DatabaseConfig Database { get; set; }
    }
}
