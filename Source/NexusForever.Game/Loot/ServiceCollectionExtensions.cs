using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Loot;
using NexusForever.Shared;

namespace NexusForever.Game.Loot
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGameLoot(this IServiceCollection sc)
        {
            sc.AddSingletonLegacy<IGlobalLootManager, GlobalLootManager>();
        }
    }
}
