using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Loot;

namespace NexusForever.Game.Abstract.Loot.Generate.Condition
{
    public interface ILootGenerationConditionManager
    {
        /// <summary>
        /// Checks if <see cref="IPlayer"/> meets the <see cref="LootConditionType"/> and condition.
        /// </summary>
        bool Meets(IPlayer player, LootConditionType type, uint condition);
    }
}
