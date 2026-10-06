using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Loot;
using NexusForever.Game.Loot.Generate;
using NexusForever.Shared;

namespace NexusForever.Game.Loot
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGameLoot(this IServiceCollection sc)
        {
            sc.AddGameLootGenerate();

            sc.AddSingletonLegacy<IGlobalLootManager, GlobalLootManager>();
        }
    }
}
