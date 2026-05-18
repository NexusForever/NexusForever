using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Static.Prerequisite;

namespace NexusForever.Game.Prerequisite.Check
{
    [PrerequisiteCheck(PrerequisiteType.Level)]
    public class PrerequisiteCheckLevel : IPrerequisiteCheck
    {
        #region Dependency Injection

        private readonly ILogger<PrerequisiteCheckLevel> log;

        public PrerequisiteCheckLevel(
            ILogger<PrerequisiteCheckLevel> log)
        {
            this.log = log;
        }

        #endregion

        public bool Meets(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            switch (comparison)
            {
                case PrerequisiteComparison.Equal:
                    return player.Level == value;
                case PrerequisiteComparison.NotEqual:
                    return player.Level != value;
                case PrerequisiteComparison.GreaterThan:
                    return player.Level > value;
                case PrerequisiteComparison.GreaterThanOrEqual:
                    return player.Level >= value;
                case PrerequisiteComparison.LessThan:
                    return player.Level < value;
                case PrerequisiteComparison.LessThanOrEqual:
                    return player.Level <= value;
                default:
                    log.LogWarning($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.Level}!");
                    return false;
            }
        }
    }
}
