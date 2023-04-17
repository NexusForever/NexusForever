using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using NexusForever.Database.Auth;
using NexusForever.Database.Configuration.Model;

namespace NexusForever.WorldServer.Design
{
    public class AuthContextFactory : IDesignTimeDbContextFactory<AuthContext>
    {
        public AuthContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("WorldServer.json")
                .Build();

            IConnectionString connectionString = configuration
                .GetSection("Database")
                .Get<DatabaseConfig>()
                .Auth;

            return new AuthContext(connectionString);
        }
    }
}
