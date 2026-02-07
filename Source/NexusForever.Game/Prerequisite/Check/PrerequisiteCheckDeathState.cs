using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Prerequisite;
using System.Xml;

namespace NexusForever.Game.Prerequisite.Check
{
    [PrerequisiteCheck(PrerequisiteType.DeathState)]
    public class PrerequisiteCheckDeathState : IPrerequisiteCheck
    {
        #region Dependency Injection
        private readonly ILogger<PrerequisiteCheckDeathState> log;
        public PrerequisiteCheckDeathState(
            ILogger<PrerequisiteCheckDeathState> log)
        {
            this.log = log;
        }
        #endregion
        /// <summary>
        /// Determines whether the specified player meets a death state prerequisite based on the provided comparison and value.
        /// </summary>
        /// <remarks>
        /// <paramref name="value">The death state value to compare against the player's current death state. Must correspond to a valid DeathState enumeration value.</param>
        /// <paramref name="objectId"/> and <paramref name="parameters"/> are not used for this prerequisite check. 
        /// </remarks>
        public bool Meets(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            switch (comparison)
            {
                case PrerequisiteComparison.Equal:
                    return !player.IsAlive &&  player.DeathState == (EntityDeathState)value;
                case PrerequisiteComparison.NotEqual:
                    return player.IsAlive || player.DeathState != (EntityDeathState)value;
                default:
                    log.LogWarning($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.DeathState}!");
                    return false;
            }
        }
    }
}