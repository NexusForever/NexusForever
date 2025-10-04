using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using NexusForever.Database;
using NexusForever.Database.Configuration.Model;
using NexusForever.Database.Group;

namespace NexusForever.Server.GroupServer.Design
{
    public class GroupContextFactory : IDesignTimeDbContextFactory<GroupContext>
    {
        public GroupContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("GroupServer.json")
                .Build();

            var connectionString = configuration
                .GetSection("Database:Group")
                .Get<DatabaseConnectionString>();

            var builder = new DbContextOptionsBuilder<GroupContext>();
            builder.UseConfiguration(connectionString);

            return new GroupContext(builder.Options);
        }
    }
}
