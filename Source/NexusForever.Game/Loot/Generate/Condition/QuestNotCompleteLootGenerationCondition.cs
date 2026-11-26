using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Loot.Generate.Condition;
using NexusForever.Game.Static.Quest;

namespace NexusForever.Game.Loot.Generate.Condition
{
    public sealed class QuestNotCompleteLootGenerationCondition : ILootGenerationCondition
    {
        public bool Meets(IPlayer player, uint condition) =>
            condition is > 0u and <= ushort.MaxValue &&
            player.QuestManager.GetQuestState((ushort)condition) != QuestState.Completed;
    }
}
