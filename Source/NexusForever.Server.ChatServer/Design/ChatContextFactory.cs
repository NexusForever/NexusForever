using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using NexusForever.Database;
using NexusForever.Database.Chat;
using NexusForever.Database.Configuration.Model;

namespace NexusForever.Server.ChatServer.Design
{
    public class ChatContextFactory : IDesignTimeDbContextFactory<ChatContext>
    {
        public ChatContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("ChatServer.json")
                .Build();

            var connectionString = configuration
                .GetSection("Database:Chat")
                .Get<DatabaseConnectionString>();

            var builder = new DbContextOptionsBuilder<ChatContext>();
            builder.UseConfiguration(connectionString);

            return new ChatContext(builder.Options);
        }
    }
}
