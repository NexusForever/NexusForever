using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Loot;
using NexusForever.Game.Abstract.Loot.Distribute;
using NexusForever.Game.Loot.Distribute;
using NexusForever.Game.Loot.Generate;
using NexusForever.Shared;

namespace NexusForever.Game.Loot
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGameLoot(this IServiceCollection sc)
        {
            sc.AddGameLootGenerate();

            sc.AddTransient<ILootDistributor, IndependentLootDistributor>();

            sc.AddTransient<ILootManager, LootManager>();
            sc.AddTransientFactory<ILootInstance, LootInstance>();
            sc.AddSingleton<ILootInstanceIdProvider, LootInstanceIdProvider>();
            sc.AddTransientFactory<ILootInstanceItem, LootInstanceItem>();
            sc.AddTransientFactory<ILootInstanceItemRoll, LootInstanceItemRoll>();
        }
    }
}
