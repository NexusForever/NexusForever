using System.Linq;
using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Game.Static.Prerequisite;
using NexusForever.Game.Static.Quest;

namespace NexusForever.Game.Prerequisite.Check
{
    [PrerequisiteCheck(PrerequisiteType.IsObjectiveActive)]
    public class PrerequisiteCheckIsObjectiveActive : IPrerequisiteCheck
    {
        #region Dependency Injection

        private readonly ILogger<PrerequisiteCheckIsObjectiveActive> log;

        public PrerequisiteCheckIsObjectiveActive(
            ILogger<PrerequisiteCheckIsObjectiveActive> log)
        {
            this.log = log;
        }

        #endregion

        public bool Meets(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            // objectId = quest id, value = objective index
            bool active = IsObjectiveActive(player, (ushort)objectId, (byte)value);

            switch (comparison)
            {
                case PrerequisiteComparison.Equal:
                    return active;
                case PrerequisiteComparison.NotEqual:
                    return !active;
                default:
                    log.LogWarning($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.IsObjectiveActive}!");
                    return false;
            }
        }

        private static bool IsObjectiveActive(IPlayer player, ushort questId, byte objectiveIndex)
        {
            IQuest quest = player.QuestManager.GetActiveQuests().FirstOrDefault(q => q.Id == questId);
            if (quest == null || quest.State != QuestState.Accepted)
                return false;

            IQuestObjective objective = quest.FirstOrDefault(o => o.Index == objectiveIndex);
            if (objective == null || objective.IsComplete())
                return false;

            // Mirror Quest.CanUpdateObjective sequential gating: prior objectives must be done.
            if (objective.ObjectiveInfo.IsSequential())
            {
                foreach (IQuestObjective prior in quest.Where(o => o.Index < objectiveIndex))
                {
                    if (!prior.IsComplete())
                        return false;
                }
            }

            return true;
        }
    }
}
