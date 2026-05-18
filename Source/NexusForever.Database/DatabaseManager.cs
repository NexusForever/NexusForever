using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using NexusForever.Database.Configuration.Model;
using NexusForever.Shared;
using NLog;

namespace NexusForever.Database
{
    public class DatabaseManager : Singleton<DatabaseManager>, IDatabaseManager
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private ImmutableDictionary<Type, IDatabase> databases;

        /// <summary>
        /// Initialise <see cref="IDatabaseManager"/> and any related resources.
        /// </summary>
        public void Initialise(IDatabaseConfig config)
        {
            log.Info("Initalising databases...");

            var builder = ImmutableDictionary.CreateBuilder<Type, IDatabase>();

            foreach (Type type in NexusForeverAssemblyHelper.GetAssemblies("NexusForever.Database").SelectMany(a => a.GetTypes()))
            {
                var attribute = type.GetCustomAttribute<DatabaseAttribute>();
                if (attribute == null)
                    continue;

                IConnectionString connectionString = config.GetConnectionString(attribute.Type);
                if (connectionString == null)
                {
                    log.Trace($"No connection string for {attribute.Type}, skipping");
                    continue;
                }

                var constructor = type.GetConstructor(Type.EmptyTypes);
                if (constructor == null)
                    continue;

                IDatabase database = (IDatabase)Activator.CreateInstance(type);
                database.Initialise(connectionString);

                builder.Add(type, database);
            }

            databases = builder.ToImmutable();

            log.Info($"Initialised {databases.Count} database(s)");
        }

        /// <summary>
        /// Apply any pending Entity Framework database migrations to the databases.
        /// </summary>
        /// <remarks>
        /// This should only be called by a single server to prevent conflicts.
        /// </remarks>
        public void Migrate()
        {
            foreach ((Type _, IDatabase database) in databases)
                database.Migrate();
        }

        public T GetDatabase<T>() where T : IDatabase
        {
            if (!databases.TryGetValue(typeof(T), out IDatabase database))
                return default;

            return (T)database;
        }
    }
}