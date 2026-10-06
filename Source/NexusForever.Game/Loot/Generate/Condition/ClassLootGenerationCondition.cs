using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Loot.Generate.Condition;
using NexusForever.Game.Static.Entity;

namespace NexusForever.Game.Loot.Generate.Condition
{
    public class ClassLootGenerationCondition : ILootGenerationCondition
    {
        /// <summary>
        /// Checks if <see cref="IPlayer"/> meets the <see cref="Class"/> condition.
        /// </summary>
        public bool Meets(IPlayer player, uint condition)
        {
            return player.Class == (Class)condition;
        }
    }
}
