using System;

namespace NexusForever.Shared.Configuration
{
    public class DatabaseConfig : IDatabaseConfiguration
    {
        public DatabaseConnectionString Auth { get; set; }
        public DatabaseConnectionString Character { get; set; }
        public DatabaseConnectionString World { get; set; }

        IConnectionString IDatabaseConfiguration.GetConnectionString(DatabaseType type)
        {
            switch (type)
            {
                case DatabaseType.Auth:
                    return Auth;
                case DatabaseType.Character:
                    return Character;
                case DatabaseType.World:
                    return World;
            }

            throw new ArgumentException($"Invalid database type: {type:G}", nameof(type));
        }
    }
}
