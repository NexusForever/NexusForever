using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Prerequisite;

namespace NexusForever.Game.Prerequisite.Check
{
    [PrerequisiteCheck(PrerequisiteType.DeathState)]
    public class PrerequisiteCheckDeathState : BasePrerequisiteHandler, IPrerequisiteCheck
    {
        #region Dependency Injection

        public PrerequisiteCheckDeathState(
            ILogger<BasePrerequisiteHandler> log)
            : base(log)
        {
        }

        #endregion

        /// <summary>
        /// Determines whether the specified player meets a death state prerequisite based on the provided comparison and value.
        /// </summary>
        /// <remarks>
        /// <paramref name="value">The death state value to compare against the player's current death state. Must correspond to a valid DeathState enumeration value.
        /// <paramref name="objectId"/> and <paramref name="parameters"/> are not used for this prerequisite check. 
        /// </remarks>
        public bool Meets(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {

            EntityDeathState? deathState = player.DeathState; // snapshot otherwise the value could change during the check

            if (deathState == null)
            {
                return comparison switch
                {
                    PrerequisiteComparison.Equal => false,              // null can not be equal to any valid value
                    PrerequisiteComparison.NotEqual => true,            // null will always be not equal to any valid value
                    PrerequisiteComparison.GreaterThan => false,        // null cannot be > a value
                    PrerequisiteComparison.GreaterThanOrEqual => false, // null cannot be >= a value
                    PrerequisiteComparison.LessThan => false,           // null cannot be < a value
                    PrerequisiteComparison.LessThanOrEqual => false,    // null cannot be <= a value
                    _ => throw new InvalidOperationException($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.DeathState}!")
                };
            }

            // player does have a deathState
            return MatchEnum(deathState.Value, (EntityDeathState) value, comparison, PrerequisiteType.DeathState);
        }
    }
}