using NexusForever.Shared.Configuration;

namespace NexusForever.WorldServer
{
    public class WorldServerConfiguration
    {
        public NetworkConfig Network { get; set; }
        public DatabaseConfig Database { get; set; }
        public bool UseCache { get; set; } = false;
        public ushort RealmId { get; set; }
    }
}
