using System;
using NexusForever.Database.Auth;
using NexusForever.Database.Character;
using NexusForever.Database.Configuration;
using NexusForever.Database.World;
using NLog;

namespace NexusForever.Shared.Database
{
    public static class DatabaseManager
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public static AuthDatabase AuthDatabase { get; private set; }
        public static CharacterDatabase CharacterDatabase { get; private set; }
        public static WorldDatabase WorldDatabase { get; private set; }

        public static void Initialise(DatabaseConfig config)
        {
            if (config.Auth != null)
                AuthDatabase = new AuthDatabase(config);

            if (config.Character != null)
                CharacterDatabase = new CharacterDatabase(config);

            if (config.World != null)
                WorldDatabase = new WorldDatabase(config);
        }

        public static void Migrate()
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
