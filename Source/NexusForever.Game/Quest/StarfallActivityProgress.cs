using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map.Instance;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Game.Static.Quest;

namespace NexusForever.Game.Quest
{
    internal static class StarfallActivityProgress
    {
        internal const uint Pvp = 1;
        internal const uint PrimeDungeon = 2;
        internal const uint Expedition = 3;

        private readonly record struct ActivityGoal(uint Id, uint Signal, uint Count);

        private static ActivityGoal? GetGoal(ushort questId) => questId switch
        {
            10504 => new(21216, Expedition, 5),
            10537 => new(21368, Expedition, 5),
            10505 => new(21217, Pvp, 3),
            10538 => new(21369, Pvp, 3),
            10506 => new(21218, PrimeDungeon, 2),
            10539 => new(21370, PrimeDungeon, 2),
            _ => null
        };

        // ActivateEntity's data in these rows identifies an activity rather than a creature.
        internal static bool OwnsObjective(ushort questId, uint objectiveId) => GetGoal(questId)?.Id == objectiveId;

        internal static void RecordCompletion(IPlayer player, IContentMapInstance map, uint activity)
        {
            if (player == null || !player.InWorld || player.IsLoading || !ReferenceEquals(player.Map, map) ||
                !player.Session.CanProcessIncomingPackets)
                return;

            bool changed = false;
            foreach (IQuest quest in player.QuestManager.GetActiveQuests())
            {
                if (quest is not { State: QuestState.Accepted, PendingDelete: false, Info.Entry.QuestCategoryId: 71 } ||
                    GetGoal(quest.Id) is not { } definition || definition.Signal != activity)
                    continue;

                IQuestObjective goal = quest.FirstOrDefault(objective => objective.ObjectiveInfo.Id == definition.Id &&
                    objective.ObjectiveInfo.Type == QuestObjectiveType.ActivateEntity &&
                    objective.ObjectiveInfo.Entry.Data == definition.Signal &&
                    objective.ObjectiveInfo.Entry.Count == definition.Count);
                if (goal == null)
                    continue;

                uint previous = goal.Progress;
                quest.ObjectiveUpdate(definition.Id, 1u);
                changed |= goal.Progress > previous;
            }

            if (changed)
                player.Save();
        }
    }
}
