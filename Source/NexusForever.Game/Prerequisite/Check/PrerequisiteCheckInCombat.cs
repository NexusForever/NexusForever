using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Static.Prerequisite;

// ObjectId is not used in this check, however gametable queries for prerequisites have shown a non zero value for objectId, if it becomes a problem investigate further
namespace NexusForever.Game.Prerequisite.Check
{
    [PrerequisiteCheck(PrerequisiteType.InCombat)]
    public class PrerequisiteCheckInCombat : BasePrerequisiteHandler, IPrerequisiteCheck
    {
        #region Dependency Injection

        public PrerequisiteCheckInCombat(ILogger<BasePrerequisiteHandler> log): base(log)
        {
        }

        #endregion

        /// <summary>
        /// Checks if the player is in combat
        /// </summary>
        /// <remarks>
        /// <param name="comparison"></param> Only equal and not equal are supported
        /// <param name="value"></param> unused
        /// <param name="objectId"></param> unused
        /// <param name="parameters"></param> unused
        /// </remarks>
        public bool Meets(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            return MatchBoolean(player.InCombat, comparison, PrerequisiteType.InCombat);
        }
    }
}
