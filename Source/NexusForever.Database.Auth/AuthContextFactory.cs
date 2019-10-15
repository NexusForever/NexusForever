using Microsoft.EntityFrameworkCore.Design;
using NexusForever.Database.Configuration;

namespace NexusForever.Database.Auth
{
    public class AuthContextFactory : IDesignTimeDbContextFactory<AuthContext>
    {
        public AuthContext CreateDbContext(string[] args)
        {
            return new AuthContext(new DatabaseConfig
            {
                Auth = new DatabaseConnectionString
                {
                    Provider         = DatabaseProvider.MySql,
                    ConnectionString = "server=127.0.0.1;port=3306;user=nexusforever;password=nexusforever;database=nexus_forever_auth"
                }
            });
        }
    }
}
