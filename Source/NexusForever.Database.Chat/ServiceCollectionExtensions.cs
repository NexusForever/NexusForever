using Microsoft.Extensions.DependencyInjection;
using NexusForever.Database.Chat.Repository;
using NexusForever.Database.Configuration.Model;

namespace NexusForever.Database.Chat
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddChatDatabase(this IServiceCollection sc, DatabaseConnectionString connectionString)
        {
            sc.AddDbContext<ChatContext>(options => options.UseConfiguration(connectionString));

            sc.AddScoped<CharacterRepository>();
            sc.AddScoped<ChatChannelRepository>();
            sc.AddScoped<InternalMessageRepository>();

            return sc;
        }
    }
}
