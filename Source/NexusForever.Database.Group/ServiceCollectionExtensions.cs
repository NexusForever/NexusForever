using Microsoft.Extensions.DependencyInjection;
using NexusForever.Database.Configuration.Model;
using NexusForever.Database.Group.Repository;

namespace NexusForever.Database.Group
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGroupDatabase(this IServiceCollection sc, DatabaseConnectionString connectionString)
        {
            sc.AddDbContext<GroupContext>(options => options.UseConfiguration(connectionString));

            sc.AddScoped<CharacterRepository>();
            sc.AddScoped<GroupRepository>();
            sc.AddScoped<InternalMessageRepository>();

            return sc;
        }
    }
}
