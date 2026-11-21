using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Loot.Generate;
using NexusForever.Game.Abstract.Loot.Generate.Condition;
using NexusForever.Game.Loot.Generate.Condition;
using NexusForever.Game.Static.Loot;
using NexusForever.Shared;

namespace NexusForever.Game.Loot.Generate
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGameLootGenerate(this IServiceCollection sc)
        {
            sc.AddSingleton<ILootGroupManager, LootGroupManager>();

            sc.AddTransientFactory<ILootGroup, LootGroup>();
            sc.AddTransientFactory<ILootItem, LootItem>();
            sc.AddTransientFactory<ILootBuilder, LootBuilder>();
            sc.AddTransientFactory<ILootBuilderItem, LootBuilderItem>();

            sc.AddSingleton<ILootGenerationConditionManager, LootGenerationConditionManager>();
            sc.AddKeyedTransient<ILootGenerationCondition, ClassLootGenerationCondition>(LootConditionType.IsClass);
        }
    }
}
