using Microsoft.EntityFrameworkCore.Design;
using NexusForever.Database.Configuration;
using NexusForever.Database.World;
using NexusForever.Shared.Configuration;

namespace NexusForever.WorldServer.Design
{
    public class WorldContextFactory : IDesignTimeDbContextFactory<WorldContext>
    {
        public WorldContext CreateDbContext(string[] args)
        {
            ConfigurationManager<WorldServerConfiguration>.Instance.Initialise("WorldServer.json");
            return new WorldContext(new DatabaseConfig
            {
                World = new DatabaseConnectionString
                {
                    Provider         = DatabaseProvider.MySql,
                    ConnectionString = ConfigurationManager<WorldServerConfiguration>.Instance.Config.Database.World.ConnectionString
                }
            });
        }
    }
}
