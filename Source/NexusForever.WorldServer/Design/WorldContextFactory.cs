using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using NexusForever.Database.Configuration.Model;
using NexusForever.Database.World;

namespace NexusForever.WorldServer.Design
{
    public class WorldContextFactory : IDesignTimeDbContextFactory<WorldContext>
    {
        public WorldContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("WorldServer.json")
                .Build();

            IConnectionString connectionString = configuration
                .GetSection("Database")
                .Get<DatabaseConfig>()
                .World;

            return new WorldContext(connectionString);
        }
    }
}
