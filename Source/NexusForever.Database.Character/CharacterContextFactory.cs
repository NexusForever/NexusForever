using Microsoft.EntityFrameworkCore.Design;
using NexusForever.Database.Configuration;

namespace NexusForever.Database.Character
{
    public class CharacterContextFactory : IDesignTimeDbContextFactory<CharacterContext>
    {
        public CharacterContext CreateDbContext(string[] args)
        {
            return new CharacterContext(new DatabaseConfig
            {
                Character = new DatabaseConnectionString
                {
                    Provider         = DatabaseProvider.MySql,
                    ConnectionString = "server=127.0.0.1;port=3306;user=nexusforever;password=nexusforever;database=nexus_forever_character"
                }
            });
        }
    }
}
