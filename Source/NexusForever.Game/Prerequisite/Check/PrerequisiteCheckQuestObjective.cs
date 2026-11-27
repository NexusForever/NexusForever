using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Static.Prerequisite;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Prerequisite.Check
{
    [PrerequisiteCheck(PrerequisiteType.QuestObjective)]
    public sealed class PrerequisiteCheckQuestObjective : IPrerequisiteCheck
    {
        private readonly IGameTableManager gameTableManager;

        public PrerequisiteCheckQuestObjective(IGameTableManager gameTableManager)
        {
            this.gameTableManager = gameTableManager;
        }

        public bool Meets(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            if (objectId is 0u or > ushort.MaxValue ||
                comparison is not (PrerequisiteComparison.Equal or PrerequisiteComparison.NotEqual))
                return false;

            Quest2Entry quest = gameTableManager.Quest2.GetEntry(objectId);
            if (quest == null || value >= quest.Objectives.Length)
                return false;

            uint objectiveId = quest.Objectives[value];
            if (objectiveId == 0u || gameTableManager.QuestObjective.GetEntry(objectiveId) == null)
                return false;

            bool completed = player.QuestManager.HasCompletedObjective((ushort)objectId, objectiveId);
            return comparison == PrerequisiteComparison.Equal ? completed : !completed;
        }
    }
}
