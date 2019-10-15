using Microsoft.EntityFrameworkCore.Design;
using NexusForever.Database.Configuration;

namespace NexusForever.Database.World
{
    public class WorldContextFactory : IDesignTimeDbContextFactory<WorldContext>
    {
        public WorldContext CreateDbContext(string[] args)
        {
            return new WorldContext(new DatabaseConfig
            {
                World = new DatabaseConnectionString
                {
                    Provider         = DatabaseProvider.MySql,
                    ConnectionString = "server=127.0.0.1;port=3306;user=nexusforever;password=nexusforever;database=nexus_forever_world"
                }
            });
        }
    }
}
