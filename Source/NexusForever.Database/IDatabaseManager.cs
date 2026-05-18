using NexusForever.Database.Configuration.Model;

namespace NexusForever.Database
{
    public interface IDatabaseManager
    {
        /// <summary>
        /// Initialise <see cref="IDatabaseManager"/> and any related resources.
        /// </summary>
        void Initialise(IDatabaseConfig config);

        /// <summary>
        /// Apply any pending Entity Framework database migrations to the databases.
        /// </summary>
        /// <remarks>
        /// This should only be called by a single server to prevent conflicts.
        /// </remarks>
        void Migrate();

        T GetDatabase<T>() where T : IDatabase;
    }
}