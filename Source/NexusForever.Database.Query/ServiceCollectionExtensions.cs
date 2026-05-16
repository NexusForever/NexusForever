using Microsoft.Extensions.DependencyInjection;
using NexusForever.Database.Configuration.Model;
using NexusForever.Database.Query.Repository;
using NexusForever.Database.Query.Repository.Query;

namespace NexusForever.Database.Query
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddQueryDatabase(this IServiceCollection sc, DatabaseConnectionString connectionString)
        {
            sc.AddDbContext<QueryContext>(options => options.UseConfiguration(connectionString));

            sc.AddScoped<CharacterRepository>();
            sc.AddScoped<QueryRepository>();
            sc.AddTransient<QueryExpressionBuilder>();

            return sc;
        }
    }
}
