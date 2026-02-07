using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Static.Prerequisite;
using NexusForever.Game.Static.Reputation;

namespace NexusForever.Game.Prerequisite.Check
{
    [PrerequisiteCheck(PrerequisiteType.Faction)]
    public class PrerequisiteCheckMutableFaction : IPrerequisiteCheck
    {
        #region Dependency Injection
        private readonly ILogger<PrerequisiteCheckMutableFaction> log;
        public PrerequisiteCheckMutableFaction(
            ILogger<PrerequisiteCheckMutableFaction> log)
        {
            this.log = log;
        }
        #endregion

        /// <summary>
        /// Determines whether the specified player meets a faction prerequisite based on the provided comparison and value.
        /// </summary>
        /// <remarks>
        /// <paramref name="value">The faction value to compare against the player's faction. Must correspond to a valid Faction enumeration value.</param>
        /// <paramref name="objectId"> and <paramref name="parameters"> are not used for this prerequisite check. </param>
        /// <\remarks>
        public bool Meets(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            switch (comparison)
            {
                case PrerequisiteComparison.Equal:
                    return player.Faction1 ==  (Faction) value;
                case PrerequisiteComparison.NotEqual:
                    return player.Faction1 != (Faction) value;
                default:
                    log.LogWarning($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.Faction}!");
                    return false;
            }
        }
    }
}