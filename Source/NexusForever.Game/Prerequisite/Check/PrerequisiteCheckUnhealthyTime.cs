using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Static.Prerequisite;

namespace NexusForever.Game.Prerequisite.Check
{
    /// <remarks>
    /// Prerequisite 37806 is the only one of this type: "You cannot recall while in Unhealthy Time" (NotEqual, object 2).
    /// It is the caster cast prerequisite of every recall: Recall - Transmat 8568, Recall - House 38408, Teleport to Thayd 70864
    /// and Illium 70865, Rapid Transport 82922 and 82956, and through 37809 the stuck window recalls 49881 and 49882.
    /// Without a check the type failed, so none of these spells could be cast.
    /// </remarks>
    [PrerequisiteCheck(PrerequisiteType.UnhealthyTime)]
    public class PrerequisiteCheckUnhealthyTime : IPrerequisiteCheck
    {
        #region Dependency Injection

        private readonly ILogger<PrerequisiteCheckUnhealthyTime> log;

        public PrerequisiteCheckUnhealthyTime(
            ILogger<PrerequisiteCheckUnhealthyTime> log)
        {
            this.log = log;
        }

        #endregion

        public bool Meets(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            // unhealthy time (play time limit) is not implemented, a player is never in it
            switch (comparison)
            {
                case PrerequisiteComparison.NotEqual:
                    return true;
                case PrerequisiteComparison.Equal:
                    return false;
                default:
                    log.LogWarning($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.UnhealthyTime}!");
                    return false;
            }
        }
    }
}
