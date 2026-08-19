using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Static.Prerequisite;

namespace NexusForever.Game.Prerequisite.Check
{
    [PrerequisiteCheck(PrerequisiteType.OtherPrerequisite)]
    public class PrerequisiteCheckOtherPrerequisite : IPrerequisiteCheck
    {
        #region Dependency Injection

        private readonly ILogger<PrerequisiteCheckOtherPrerequisite> log;
        private readonly IPrerequisiteManager prerequisiteManager;

        public PrerequisiteCheckOtherPrerequisite(
            ILogger<PrerequisiteCheckOtherPrerequisite> log,
            IPrerequisiteManager prerequisiteManager)
        {
            this.log                 = log;
            this.prerequisiteManager = prerequisiteManager;
        }

        #endregion

        public bool Meets(IUnitEntity subject, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            switch (comparison)
            {
                case PrerequisiteComparison.NotEqual:
                    return !prerequisiteManager.Meets(subject, objectId, parameters);
                case PrerequisiteComparison.Equal:
                    return prerequisiteManager.Meets(subject, objectId, parameters);
                default:
                    log.LogWarning($"Unhandled {comparison} for {PrerequisiteType.OtherPrerequisite}!");
                    return false;
            }
        }
    }
}
