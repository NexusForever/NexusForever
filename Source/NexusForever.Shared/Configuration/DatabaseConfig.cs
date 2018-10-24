using System;
using Newtonsoft.Json;

namespace NexusForever.Shared.Configuration
{
    public class DatabaseConfig : IDatabaseConfiguration
    {
        [JsonConverter(typeof(ConnectionStringConverter))]
        public IConnectionString Auth { get; set; }
        [JsonConverter(typeof(ConnectionStringConverter))]
        public IConnectionString Character { get; set; }
        [JsonConverter(typeof(ConnectionStringConverter))]
        public IConnectionString World { get; set; }


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
