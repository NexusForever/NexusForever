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

        /// <summary>
        /// Initialise <see cref="DatabaseManager"/> and any related resources.
        /// </summary>
        public void Initialise(DatabaseConfig config)
        {
            if (AuthDatabase != null || CharacterDatabase != null || WorldDatabase != null)
                throw new InvalidOperationException();

            log.Info("Initialising database manager...");

            if (config.Auth != null)
            {
                AuthDatabase = new AuthDatabase(config);
                log.Info("Initialising auth database...");
            }

            if (config.Character != null)
            {
                CharacterDatabase = new CharacterDatabase(config);
                log.Info("Initialising character database...");
            }

            if (config.World != null)
            {
                WorldDatabase = new WorldDatabase(config);
                log.Info("Initialising world database...");
            }
        }

        /// <summary>
        /// Apply any pending Entity Framework database migrations to the databases.
        /// </summary>
        /// <remarks>
        /// This should only be called by a single server to prevent conflicts.
        /// </remarks>
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
