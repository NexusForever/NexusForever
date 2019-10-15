using NexusForever.Database.Configuration;
using NexusForever.Shared.Configuration;

namespace NexusForever.AuthServer
{
    public class AuthServerConfiguration
    {
        public NetworkConfig Network { get; set; }
        public DatabaseConfig Database { get; set; }
    }
}
