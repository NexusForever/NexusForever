using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Static.Prerequisite;
using NexusForever.Game.Static.Reputation;

namespace NexusForever.Game.Prerequisite.Check
{
    [PrerequisiteCheck(PrerequisiteType.Faction)]
    public class PrerequisiteCheckMutableFaction : BasePrerequisiteHandler, IPrerequisiteCheck
    {
        #region dependency injection
        public PrerequisiteCheckMutableFaction(ILogger<BasePrerequisiteHandler> log): base(log){}
        #endregion

        /// <summary>
        /// Determines whether the specified player meets a faction prerequisite based on the provided comparison and value.
        /// </summary>
        /// <remarks>
        /// <param name="value">The faction value to compare against the player's faction. Must correspond to a valid Faction enumeration value.</param>
        /// <param name="objectId"> and <paramref name="parameters"> are not used for this prerequisite check. </param>
        /// </remarks>
        public bool Meets(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            return MatchEnum(player.Faction1, (Faction)value, comparison, PrerequisiteType.Faction);
        }
    }
}