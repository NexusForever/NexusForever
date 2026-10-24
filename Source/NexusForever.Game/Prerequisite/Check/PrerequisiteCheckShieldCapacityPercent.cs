using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Prerequisite;

namespace NexusForever.Game.Prerequisite.Check
{
    [PrerequisiteCheck(PrerequisiteType.ShieldCapacityPercent)]
    public class PrerequisiteCheckShieldCapacityPercent : IPrerequisiteCheck
    {
        #region Dependency Injection

        private readonly ILogger<PrerequisiteCheckShieldCapacityPercent> log;

        public PrerequisiteCheckShieldCapacityPercent(
            ILogger<PrerequisiteCheckShieldCapacityPercent> log)
        {
            this.log = log;
        }

        #endregion

        /// <summary>
        /// The same check as <see cref="PrerequisiteCheckVitalPercent"/> with the vital fixed to
        /// <see cref="Vital.ShieldCapacity"/>: the client's handler ignores objectId entirely and reads the
        /// unit's current and maximum shield capacity through the same two accessors the vital lookup uses
        /// for that vital. value is a percentage and all six comparisons are honoured.
        /// </summary>
        /// <remarks>
        /// objectId is not read, which the shipped data agrees with: it is zero in 25 of 34 rows, and 3
        /// (<see cref="Vital.ShieldCapacity"/>) in all 9 of the rest. The unguarded division is load-bearing
        /// here - see the remarks on <see cref="PrerequisiteCheckVitalPercent"/>.
        /// </remarks>
        public bool Meets(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            return VitalPercentage.Meets(log, player, comparison, value, Vital.ShieldCapacity, PrerequisiteType.ShieldCapacityPercent);
        }
    }
}
