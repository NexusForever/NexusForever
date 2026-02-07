using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Static.Prerequisite;
using NexusForever.Game.Static.Reputation;

namespace NexusForever.Game.Prerequisite.Check
{
    [PrerequisiteCheck(PrerequisiteType.Reputation)]
    public class PrerequisiteCheckMutableFactionReputationLevel : IPrerequisiteCheck
    {
        #region Dependency Injection
        private readonly ILogger<PrerequisiteCheckMutableFaction> log;
        public PrerequisiteCheckMutableFactionReputationLevel(
            ILogger<PrerequisiteCheckMutableFaction> log)
        {
            this.log = log;
        }
        #endregion

        /// <summary>
        /// Checks a player's mutable faction reputation level against a specified value.
        /// </summary>
        /// <remarks>
        /// This prerequisite check evaluates the player's disposition toward their primary faction
        /// <paramref name="value"/> is enum value of <see cref="Disposition"/>.
        /// <paramref name =objectId"/> is ignored for this check. 
        /// </remarks>
        public bool Meets(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            Disposition playerReputation = player.GetDispositionTo(player.Faction1);
            switch (comparison)
            {
                case PrerequisiteComparison.Equal:
                    return playerReputation == (Disposition) value;
                case PrerequisiteComparison.NotEqual:
                    return playerReputation != (Disposition) value;
                case PrerequisiteComparison.GreaterThanOrEqual:
                    return playerReputation >= (Disposition) value;
                case PrerequisiteComparison.GreaterThan:
                    return playerReputation >  (Disposition) value;
                case PrerequisiteComparison.LessThanOrEqual:
                    return playerReputation <= (Disposition) value;
                case PrerequisiteComparison.LessThan:
                    return playerReputation <  (Disposition) value;

                default:
                    log.LogWarning($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.Reputation}!");
                    return false;
            }
        }
    }
}