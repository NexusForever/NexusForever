using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using NexusForever.Database;
using NexusForever.Database.Configuration.Model;
using NexusForever.Database.Friendship;

namespace NexusForever.Server.Friendship.Design
{
    public class FriendshipContextFactory : IDesignTimeDbContextFactory<FriendshipContext>
    {
        public FriendshipContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("FriendshipServer.json")
                .Build();

            var connectionString = configuration
                .GetSection("Database:Friendship")
                .Get<DatabaseConnectionString>();

            var builder = new DbContextOptionsBuilder<FriendshipContext>();
            builder.UseConfiguration(connectionString);

            return new FriendshipContext(builder.Options);
        }
    }
}
