using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Prerequisite;

namespace NexusForever.Game.Prerequisite.Check
{
    [PrerequisiteCheck(PrerequisiteType.Gender)]
    public class PrerequisiteCheckGender : IPrerequisiteCheck
    {
        #region Dependency Injection

        private readonly ILogger<PrerequisiteCheckGender> log;

        public PrerequisiteCheckGender(
            ILogger<PrerequisiteCheckGender> log)
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
                case PrerequisiteComparison.NotEqual:
                    return player.Sex != (Sex)value;
                case PrerequisiteComparison.Equal:
                    return player.Sex == (Sex)value;
                default:
                    log.LogWarning($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.Gender}!");
                    return false;
            }
        }
    }
}
