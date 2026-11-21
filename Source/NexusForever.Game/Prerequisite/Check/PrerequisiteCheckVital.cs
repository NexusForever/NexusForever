using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Prerequisite;

namespace NexusForever.Game.Prerequisite.Check
{
    [PrerequisiteCheck(PrerequisiteType.Vital)]
    public class PrerequisiteCheckVital : IPrerequisiteCheck
    {
        #region Dependency Injection

        private readonly ILogger<PrerequisiteCheckVital> log;

        public PrerequisiteCheckVital(
            ILogger<PrerequisiteCheckVital> log)
        {
            this.log = log;
        }

        #endregion

        public bool Meets(IUnitEntity subject, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            switch (comparison)
            {
                case PrerequisiteComparison.Equal:
                    return subject.GetVitalValue((Vital)objectId) == value;
                case PrerequisiteComparison.NotEqual:
                    return subject.GetVitalValue((Vital)objectId) != value;
                case PrerequisiteComparison.GreaterThanOrEqual:
                    return subject.GetVitalValue((Vital)objectId) >= value;
                case PrerequisiteComparison.GreaterThan:
                    return subject.GetVitalValue((Vital)objectId) > value;
                case PrerequisiteComparison.LessThanOrEqual:
                    return subject.GetVitalValue((Vital)objectId) <= value;
                case PrerequisiteComparison.LessThan:
                    return subject.GetVitalValue((Vital)objectId) < value;
                default:
                    log.LogWarning($"Unhandled {comparison} for {PrerequisiteType.Vital}!");
                    return false;
            }
        }
    }
}
