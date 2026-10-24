using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Prerequisite;

namespace NexusForever.Game.Prerequisite.Check
{
    [PrerequisiteCheck(PrerequisiteType.VitalPercent)]
    public class PrerequisiteCheckVitalPercent : IPrerequisiteCheck
    {
        #region Dependency Injection

        private readonly ILogger<PrerequisiteCheckVitalPercent> log;

        public PrerequisiteCheckVitalPercent(
            ILogger<PrerequisiteCheckVitalPercent> log)
        {
            this.log = log;
        }

        #endregion

        /// <summary>
        /// objectId is a <see cref="Vital"/> and value is a percentage 0-100. The client computes
        /// current * 100.0f / max in single precision, in that order, and compares the result with value
        /// through its float comparator, which honours all six comparisons.
        /// </summary>
        /// <remarks>
        /// The division is deliberately left unguarded, because the client leaves it unguarded and the
        /// shipped data depends on the result. With a maximum of 0 and a current of 0 the quotient is NaN,
        /// so only NotEqual is met; with a current above 0 it is +infinity, so GreaterThan and
        /// GreaterThanOrEqual are met. <see cref="PrerequisiteType.ShieldCapacityPercent"/> relies on that:
        /// its only shipped user reads "shield percentage >= 0", which is the client's way of asking
        /// whether the unit has a shield at all.
        ///
        /// The client returns false for a null unit for every comparison, including NotEqual, because the
        /// null test precedes the comparison. <see cref="IPlayer"/> is never null here, so that path has no
        /// counterpart.
        /// </remarks>
        public bool Meets(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            return VitalPercentage.Meets(log, player, comparison, value, (Vital)objectId, PrerequisiteType.VitalPercent);
        }
    }
}
