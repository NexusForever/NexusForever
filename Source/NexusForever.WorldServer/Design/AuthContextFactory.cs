using Microsoft.EntityFrameworkCore.Design;
using NexusForever.Database.Auth;
using NexusForever.Database.Configuration.Model;
using NexusForever.Shared.Configuration;

namespace NexusForever.WorldServer.Design
{
    public class AuthContextFactory : IDesignTimeDbContextFactory<AuthContext>
    {
        public AuthContext CreateDbContext(string[] args)
        {
            SharedConfiguration.Instance.Initialise<WorldServerConfiguration>("WorldServer.json");
            return new AuthContext(SharedConfiguration.Instance.Get<DatabaseConfig>().Auth);
        }
    }
}
