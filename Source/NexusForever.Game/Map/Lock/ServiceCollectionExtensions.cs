using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Map.Lock;
using NexusForever.Shared;

namespace NexusForever.Game.Map.Lock
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGameMapLock(this IServiceCollection sc)
        {
            sc.AddSingletonLegacy<IMapLockManager, MapLockManager>();
            sc.AddTransientFactory<IMapLockCollection, MapLockCollection>();

            sc.AddSingleton<IFactoryInterface<IMapLock>, FactoryInterface<IMapLock>>();
            sc.AddTransient<IMapLock, MapLock>();
            sc.AddTransient<IResidenceMapLock, ResidenceMapLock>();
        }
    }
}
