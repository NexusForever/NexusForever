using Microsoft.Extensions.DependencyInjection;
using NexusForever.Shared;

namespace NexusForever.Database
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDatabase(this IServiceCollection sc)
        {
            sc.AddSingletonLegacy<IDatabaseManager, DatabaseManager>();
        }
    }
}
