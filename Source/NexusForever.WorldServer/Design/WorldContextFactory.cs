using Microsoft.EntityFrameworkCore.Design;
using NexusForever.Database.Configuration.Model;
using NexusForever.Database.World;
using NexusForever.Shared.Configuration;

namespace NexusForever.WorldServer.Design
{
    public class WorldContextFactory : IDesignTimeDbContextFactory<WorldContext>
    {
        public WorldContext CreateDbContext(string[] args)
        {
            SharedConfiguration.Instance.Initialise<WorldServerConfiguration>("WorldServer.json");
            return new WorldContext(SharedConfiguration.Instance.Get<DatabaseConfig>().World);
        }
    }
}
