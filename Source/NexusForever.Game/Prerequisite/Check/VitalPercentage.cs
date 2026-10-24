using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Prerequisite;

namespace NexusForever.Game.Prerequisite.Check
{
    /// <summary>
    /// Shared arithmetic for the prerequisite types that compare a <see cref="Vital"/> against a
    /// percentage of its maximum: <see cref="PrerequisiteType.VitalPercent"/> (the vital comes from the
    /// row) and <see cref="PrerequisiteType.ShieldCapacityPercent"/> (the vital is fixed).
    /// </summary>
    internal static class VitalPercentage
    {
        /// <summary>
        /// current * 100.0f / max, in single precision and in that order, compared with value.
        /// </summary>
        public static bool Meets(ILogger log, IPlayer player, PrerequisiteComparison comparison, uint value, Vital vital, PrerequisiteType type)
        {
            float percent = GetCurrent(player, vital) * 100f / GetMaximum(player, vital);

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
                    log.LogWarning($"Unhandled PrerequisiteComparison {comparison} for {type}!");
                    return false;
            }
        }

        /// <summary>
        /// The current value of the <see cref="Vital"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="IWorldEntity.GetVitalValue"/> resolves every vital that carries a VitalAttribute, but
        /// Health and ShieldCapacity do not carry one - they are integer properties of their own - so they
        /// are read directly. Every other vital goes through the existing lookup, which answers 0 for a
        /// vital it has no getter for.
        /// </remarks>
        private static float GetCurrent(IPlayer player, Vital vital)
        {
            switch (vital)
            {
                case Vital.Health:
                    return player.Health;
                case Vital.ShieldCapacity:
                    return player.Shield;
                default:
                    return player.GetVitalValue(vital);
            }
        }

        /// <summary>
        /// The unit's own maximum for the <see cref="Vital"/>, taken from the same properties the client's
        /// maximum accessor reads for each vital. A vital with no maximum answers 0, which makes the
        /// quotient NaN or infinite exactly as it does in the client.
        /// </summary>
        private static float GetMaximum(IPlayer player, Vital vital)
        {
            switch (vital)
            {
                case Vital.Health:
                    return player.MaxHealth;
                case Vital.ShieldCapacity:
                    return player.MaxShieldCapacity;
                case Vital.Endurance:
                    return player.GetPropertyValue(Property.ResourceMax0);
                case Vital.Resource1:
                    return player.GetPropertyValue(Property.ResourceMax1);
                case Vital.Resource2:
                    return player.GetPropertyValue(Property.ResourceMax2);
                case Vital.Resource3:
                    return player.GetPropertyValue(Property.ResourceMax3);
                case Vital.Resource4:
                    return player.GetPropertyValue(Property.ResourceMax4);
                case Vital.Resource5:
                    return player.GetPropertyValue(Property.ResourceMax5);
                case Vital.Resource6:
                    return player.GetPropertyValue(Property.ResourceMax6);
                case Vital.Focus:
                    return player.GetPropertyValue(Property.BaseFocusPool);
                case Vital.Dash:
                    return player.GetPropertyValue(Property.ResourceMax7);
                default:
                    return 0f;
            }
        }
    }
}
