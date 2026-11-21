using Microsoft.Extensions.DependencyInjection;
using NexusForever.Database.Configuration.Model;
using NexusForever.Database.Friendship.Repository;

namespace NexusForever.Database.Friendship
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFriendshipDatabase(this IServiceCollection sc, DatabaseConnectionString connectionString)
        {
            sc.AddDbContext<FriendshipContext>(options => options.UseConfiguration(connectionString));

            sc.AddScoped<AccountRepository>();
            sc.AddScoped<CharacterRepository>();
            sc.AddScoped<AccountFriendRepository>();
            sc.AddScoped<FriendRepository>();
            sc.AddScoped<InternalMessageRepository>();

            return sc;
        }
    }
}
