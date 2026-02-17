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

        // This prerequisite can not be refactored into the BasePrerequisiteHandler pattern as this is used for checking other prerequisites and not for handling them.
        public bool Meets(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            switch (comparison)
            {
                case PrerequisiteComparison.NotEqual:
                    return !prerequisiteManager.Meets(player, objectId);
                case PrerequisiteComparison.Equal:
                    return prerequisiteManager.Meets(player, objectId);
                default:
                    log.LogWarning($"Unhandled {comparison} for {PrerequisiteType.OtherPrerequisite}!");
                    return false;
            }
        }
    }
}
