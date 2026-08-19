using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Static.Prerequisite;

namespace NexusForever.Game.Prerequisite.Check
{
    [PrerequisiteCheck(PrerequisiteType.Path)]
    public class PrerequisiteCheckPath : IPrerequisiteCheck
    {
        #region Dependency Injection

        private readonly ILogger<PrerequisiteCheckPath> log;

        public PrerequisiteCheckPath(
            ILogger<PrerequisiteCheckPath> log)
        {
            this.log = log;
        }

        #endregion

        public bool Meets(IUnitEntity subject, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            if (subject is not IPlayer player)
                return false;

            switch (comparison)
            {
                case PrerequisiteComparison.Equal:
                    return player.PathManager.IsPathActive((Static.Entity.Path)value);
                default:
                    log.LogWarning($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.Path}!");
                    return false;
            }
        }
    }
}
