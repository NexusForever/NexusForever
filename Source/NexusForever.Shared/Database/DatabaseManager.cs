using System;
using NexusForever.Database.Auth;
using NexusForever.Database.Character;
using NexusForever.Database.Configuration;
using NexusForever.Database.World;
using NLog;

namespace NexusForever.Shared.Database
{
    public class DatabaseManager : Singleton<DatabaseManager>
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public AuthDatabase AuthDatabase { get; private set; }
        public CharacterDatabase CharacterDatabase { get; private set; }
        public WorldDatabase WorldDatabase { get; private set; }

        private DatabaseManager()
        {
        }

        public void Initialise(DatabaseConfig config)
        {
            if (config.Auth != null)
                AuthDatabase = new AuthDatabase(config);

            if (config.Character != null)
                CharacterDatabase = new CharacterDatabase(config);

            if (config.World != null)
                WorldDatabase = new WorldDatabase(config);
        }

        public void Migrate()
        {
            log.Info("Applying database migrations...");

            try
            {
                AuthDatabase.Migrate();
                CharacterDatabase.Migrate();
                WorldDatabase.Migrate();
            }
            catch (Exception exception)
            {
                log.Fatal(exception);
                throw;
            }
        }
    }
}
