using NexusForever.Game.Abstract.Entity;

namespace NexusForever.Game.Abstract.Loot.Generate.Condition
{
    public interface ILootGenerationCondition
    {
        /// <summary>
        /// Checks if <see cref="IPlayer"/> meets the condition.
        /// </summary>
        bool Meets(IPlayer player, uint condition);
    }
}
