using NexusForever.Database.Auth;
using NexusForever.Database.Character;
using NexusForever.Database.Configuration;
using NexusForever.Database.World;
using NLog;
using System;

namespace NexusForever.Shared.Database
{
    public class DatabaseManager : Singleton<DatabaseManager>, IShutdownAble
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public AuthDatabase AuthDatabase { get; private set; }
        public CharacterDatabase CharacterDatabase { get; private set; }
        public WorldDatabase WorldDatabase { get; private set; }

        private DatabaseManager()
        {
        }

        public DatabaseManager Initialise(DatabaseConfig config)
        {
            if (config.Auth != null)
                AuthDatabase = new AuthDatabase(config);

            if (config.Character != null)
                CharacterDatabase = new CharacterDatabase(config);

            if (config.World != null)
                WorldDatabase = new WorldDatabase(config);

            return Instance;
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

        /// <inheritdoc />
        public void Shutdown()
        {
            
        }
    }
}
