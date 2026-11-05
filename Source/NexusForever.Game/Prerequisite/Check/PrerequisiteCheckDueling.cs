using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Static.Prerequisite;

namespace NexusForever.Game.Prerequisite.Check
{
    /// <remarks>
    /// Used with NotEqual and object 0 by 30178 "You can't use this while a duel is in progress", which 37809 requires for the
    /// stuck window recalls 49881 and 49882. Without a check the type failed and the stuck window recalls could not be cast.
    /// </remarks>
    [PrerequisiteCheck(PrerequisiteType.Dueling)]
    public class PrerequisiteCheckDueling : IPrerequisiteCheck
    {
        #region Dependency Injection

        private readonly ILogger<PrerequisiteCheckDueling> log;

        public PrerequisiteCheckDueling(
            ILogger<PrerequisiteCheckDueling> log)
        {
            this.log = log;
        }

        #endregion

        public bool Meets(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            // duels are not implemented, a player is never in one
            switch (comparison)
            {
                case PrerequisiteComparison.NotEqual:
                    return true;
                case PrerequisiteComparison.Equal:
                    return false;
                default:
                    log.LogWarning($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.Dueling}!");
                    return false;
            }
        }
    }
}
