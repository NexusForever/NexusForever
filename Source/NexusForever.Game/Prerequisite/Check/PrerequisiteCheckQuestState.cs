using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Static.Prerequisite;
using NexusForever.Game.Static.Quest;

namespace NexusForever.Game.Prerequisite.Check
{
    [PrerequisiteCheck(PrerequisiteType.QuestState)]
    public class PrerequisiteCheckQuestState : IPrerequisiteCheck
    {
        #region Dependency Injection

        private readonly ILogger<PrerequisiteCheckQuestState> log;

        public PrerequisiteCheckQuestState(
            ILogger<PrerequisiteCheckQuestState> log)
        {
            this.log = log;
        }

        #endregion

        public bool Meets(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            if (objectId is 0u or > ushort.MaxValue || value > (uint)QuestState.Ignored)
                return false;

            QuestState state = player.QuestManager.GetQuestState((ushort)objectId) ?? QuestState.Unknown;
            switch (comparison)
            {
                case PrerequisiteComparison.Equal:
                    return state == (QuestState)value;
                case PrerequisiteComparison.NotEqual:
                    return state != (QuestState)value;
                default:
                    log.LogWarning($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.QuestState}!");
                    return false;
            }
        }
    }
}
