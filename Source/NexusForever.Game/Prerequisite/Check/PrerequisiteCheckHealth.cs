using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Static.Prerequisite;

namespace NexusForever.Game.Prerequisite.Check
{
    [PrerequisiteCheck(PrerequisiteType.Health)]
    public class PrerequisiteCheckHealth : IPrerequisiteCheck
    {
        #region Dependency Injection

        private readonly ILogger<PrerequisiteCheckHealth> log;

        public PrerequisiteCheckHealth(
            ILogger<PrerequisiteCheckHealth> log)
        {
            this.log = log;
        }

        #endregion

        /// <summary>
        /// The client compares the unit's health as a percentage of its base health against the row's value,
        /// as a float: health * 100.0f / baseHealth, then value. <see cref="IWorldEntity.MaxHealth"/> is
        /// Property.BaseHealth, which is the same divisor.
        /// </summary>
        /// <remarks>
        /// The client guards neither against a zero divisor nor against a value above 100; it does return
        /// false for a missing unit. A base health of 0 is treated as "not met" here rather than dividing.
        /// </remarks>
        public bool Meets(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            if (player.MaxHealth == 0)
                return false;

            float percent = player.Health * 100f / player.MaxHealth;

            switch (comparison)
            {
                case PrerequisiteComparison.Equal:
                    return percent == value;
                case PrerequisiteComparison.NotEqual:
                    return percent != value;
                case PrerequisiteComparison.GreaterThan:
                    return percent > value;
                case PrerequisiteComparison.GreaterThanOrEqual:
                    return percent >= value;
                case PrerequisiteComparison.LessThan:
                    return percent < value;
                case PrerequisiteComparison.LessThanOrEqual:
                    return percent <= value;
                default:
                    log.LogWarning($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.Health}!");
                    return false;
            }
        }
    }
}
