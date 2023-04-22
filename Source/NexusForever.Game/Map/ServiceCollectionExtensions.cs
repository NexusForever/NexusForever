using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Map;
using NexusForever.Shared;

namespace NexusForever.Game.Map
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGameMap(this IServiceCollection sc)
        {
            sc.AddSingletonLegacy<IEntityCacheManager, EntityCacheManager>();
            sc.AddSingletonLegacy<IMapIOManager, MapIOManager>();
            sc.AddSingletonLegacy<IMapManager, MapManager>();
        }
    }
}
