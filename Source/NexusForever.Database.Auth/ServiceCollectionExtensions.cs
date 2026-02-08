using Microsoft.Extensions.DependencyInjection;
using NexusForever.Database.Auth.Repository;
using NexusForever.Database.Configuration.Model;

namespace NexusForever.Database.Auth
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAuthDatabase(this IServiceCollection sc, DatabaseConnectionString connectionString)
        {
            sc.AddDbContext<AuthContext>(options => options.UseConfiguration(connectionString));
            sc.AddScoped<ServerRepository>();
            return sc;
        }
    }
}
