using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using NexusForever.Database;
using NexusForever.Database.Configuration.Model;
using NexusForever.Database.Query;

namespace NexusForever.Server.Character.Design
{
    public class QueryContextFactory : IDesignTimeDbContextFactory<QueryContext>
    {
        public QueryContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("CharacterServer.json")
                .Build();

            var connectionString = configuration
                .GetSection("Database:Query")
                .Get<DatabaseConnectionString>();

            var builder = new DbContextOptionsBuilder<QueryContext>();
            builder.UseConfiguration(connectionString);

            return new QueryContext(builder.Options);
        }
    }
}
