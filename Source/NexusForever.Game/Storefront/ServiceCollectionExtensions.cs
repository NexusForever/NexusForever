using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Storefront;
using NexusForever.Shared;

namespace NexusForever.Game.Storefront
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGameStore(this IServiceCollection sc)
        {
            sc.AddSingletonLegacy<IGlobalStorefrontManager, GlobalStorefrontManager>();
        }
    }
}
