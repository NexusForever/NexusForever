using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Loot.Generate.Condition;

namespace NexusForever.Game.Loot.Generate.Condition
{
    public sealed class QuestObjectiveLootGenerationCondition : ILootGenerationCondition
    {
        /// <summary>
        /// Checks whether the player can advance the specified objective table entry.
        /// </summary>
        public bool Meets(IPlayer player, uint condition)
        {
            return player.QuestManager.GetActiveQuests().Any(quest =>
                quest.Any(objective => objective.ObjectiveInfo.Id == condition &&
                    quest.IsObjectiveActive(objective.Index)));
        }
    }
}
