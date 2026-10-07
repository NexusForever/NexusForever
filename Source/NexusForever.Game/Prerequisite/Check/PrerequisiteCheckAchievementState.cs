using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Static.Prerequisite;

namespace NexusForever.Game.Prerequisite.Check
{
    [PrerequisiteCheck(PrerequisiteType.AchievementState)]
    public class PrerequisiteCheckAchievementState : BasePrerequisiteHandler, IPrerequisiteCheck
    {
        #region Dependency Injection

        public PrerequisiteCheckAchievementState(
            ILogger<BasePrerequisiteHandler> log)
            : base(log)
        {
        }

        #endregion

        public bool Meets(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            bool hasCompletedAchievement = player.AchievementManager.HasCompletedAchievement((ushort)objectId);
            return MatchBoolean(hasCompletedAchievement, comparison, PrerequisiteType.AchievementState);
        }
    }
}
