using Microsoft.EntityFrameworkCore.Design;
using NexusForever.Database.Character;
using NexusForever.Database.Configuration;
using NexusForever.Shared.Configuration;

namespace NexusForever.WorldServer.Design
{
    public class CharacterContextFactory : IDesignTimeDbContextFactory<CharacterContext>
    {
        public CharacterContext CreateDbContext(string[] args)
        {
            ConfigurationManager<WorldServerConfiguration>.Instance.Initialise("WorldServer.json");
            return new CharacterContext(new DatabaseConfig
            {
                Character = new DatabaseConnectionString
                {
                    Provider         = DatabaseProvider.MySql,
                    ConnectionString = ConfigurationManager<WorldServerConfiguration>.Instance.Config.Database.Auth.ConnectionString
                }
            });
        }
    }
}
