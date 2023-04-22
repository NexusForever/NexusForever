using Microsoft.Extensions.DependencyInjection;
using NexusForever.Shared;

namespace NexusForever.GameTable
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGameTable(this IServiceCollection sc)
        {
            sc.AddSingletonLegacy<IGameTableManager, GameTableManager>();
        }
    }
}
