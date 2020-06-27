using System;

namespace NexusForever.Database.Configuration
{
    public class DatabaseConfig : IDatabaseConfig
    {
        public DatabaseConnectionString Auth { get; set; }
        public DatabaseConnectionString Character { get; set; }
        public DatabaseConnectionString World { get; set; }

        public IConnectionString GetConnectionString(DatabaseType type)
        {
            return type switch
            {
                DatabaseType.Auth => Auth,
                DatabaseType.Character => Character,
                DatabaseType.World => World,
                _ => throw new ArgumentException($"Invalid database type: {type:G}", nameof(type))
            };
        }
    }
}
