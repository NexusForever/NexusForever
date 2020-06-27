using Microsoft.EntityFrameworkCore.Design;
using NexusForever.Database.Auth;
using NexusForever.Database.Configuration;
using NexusForever.Shared.Configuration;

namespace NexusForever.WorldServer.Design
{
    public class AuthContextFactory : IDesignTimeDbContextFactory<AuthContext>
    {
        public AuthContext CreateDbContext(string[] args)
        {
            ConfigurationManager<WorldServerConfiguration>.Instance.Initialise("WorldServer.json");
            return new AuthContext(new DatabaseConfig
            {
                Auth = new DatabaseConnectionString
                {
                    Provider         = DatabaseProvider.MySql,
                    ConnectionString = ConfigurationManager<WorldServerConfiguration>.Instance.Config.Database.Auth.ConnectionString
                }
            });
        }
    }
}
