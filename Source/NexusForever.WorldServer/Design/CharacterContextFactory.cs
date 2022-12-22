using Microsoft.EntityFrameworkCore.Design;
using NexusForever.Database.Character;
using NexusForever.Database.Configuration.Model;
using NexusForever.Shared.Configuration;

namespace NexusForever.WorldServer.Design
{
    public class CharacterContextFactory : IDesignTimeDbContextFactory<CharacterContext>
    {
        public CharacterContext CreateDbContext(string[] args)
        {
            SharedConfiguration.Instance.Initialise<WorldServerConfiguration>("WorldServer.json");
            return new CharacterContext(SharedConfiguration.Instance.Get<DatabaseConfig>().Character);
        }
    }
}
