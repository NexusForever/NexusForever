using NexusForever.Shared.Configuration;

namespace NexusForever.Shared.Database
{
    public static class DatabaseManager
    {
        public static DatabaseConfig Config { get; private set; }

        public static void Initialise(DatabaseConfig config)
        {
            Config = config;
        }
    }
}
