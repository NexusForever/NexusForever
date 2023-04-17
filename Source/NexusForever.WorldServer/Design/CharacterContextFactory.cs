using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using NexusForever.Database.Character;
using NexusForever.Database.Configuration.Model;

namespace NexusForever.WorldServer.Design
{
    public class CharacterContextFactory : IDesignTimeDbContextFactory<CharacterContext>
    {
        public CharacterContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("WorldServer.json")
                .Build();

            IConnectionString connectionString = configuration
                .GetSection("Database")
                .Get<DatabaseConfig>()
                .Character;

            return new CharacterContext(connectionString);
        }
    }
}
