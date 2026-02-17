using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Static.Prerequisite;
using NexusForever.Game.Static.Quest;

namespace NexusForever.Game.Prerequisite.Check
{
    [PrerequisiteCheck(PrerequisiteType.QuestState)]
    public class PrerequisiteCheckQuestState : BasePrerequisiteHandler, IPrerequisiteCheck
    {
        #region Dependency Injection

        public PrerequisiteCheckQuestState(
            ILogger<BasePrerequisiteHandler> log)
            : base(log)
        {
        }

        #endregion

        public bool Meets(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            QuestState? questState = player.QuestManager.GetQuestState((ushort)objectId);
            if (questState == null)
                return false;

            return MatchEnum(questState.Value, (QuestState)value, comparison, PrerequisiteType.QuestState);
        }
    }
}
