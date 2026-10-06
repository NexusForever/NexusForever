using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Loot.Generate.Condition;
using NexusForever.Game.Static.Loot;

namespace NexusForever.Game.Loot.Generate.Condition
{
    public class LootGenerationConditionManager : ILootGenerationConditionManager
    {
        #region Dependency Injection

        private readonly IServiceProvider serviceProvider;

        public LootGenerationConditionManager(
            IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        #endregion

        /// <summary>
        /// Checks if <see cref="IPlayer"/> meets the <see cref="LootConditionType"/> and condition.
        /// </summary>
        public bool Meets(IPlayer player, LootConditionType type, uint condition)
        {
            ILootGenerationCondition handler = serviceProvider.GetKeyedService<ILootGenerationCondition>(type);
            if (handler == null)
                return false;

            return handler.Meets(player, condition);
        }
    }
}
