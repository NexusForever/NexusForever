using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Static.Prerequisite;

namespace NexusForever.Game.Prerequisite.Check
{
    /// <summary>
    /// Is the <see cref="PrerequisiteEntry"/> object id an unlocked inlaid Eldan augmentation?
    /// </summary>
    [PrerequisiteCheck(PrerequisiteType.EldanAugmentation231)]
    public class PrerequisiteCheckEldanAugmentation231 : IPrerequisiteCheck
    {
        #region Dependency Injection

        private readonly ILogger<PrerequisiteCheckEldanAugmentation231> log;

        public PrerequisiteCheckEldanAugmentation231(
            ILogger<PrerequisiteCheckEldanAugmentation231> log)
        {
            this.log = log;
        }

        #endregion

        public bool Meets(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            switch (comparison)
            {
                case PrerequisiteComparison.Equal:
                    return player.SpellManager.HasUnlockedInlaidEldanAugmentation((ushort)objectId);
                case PrerequisiteComparison.NotEqual:
                    return !player.SpellManager.HasUnlockedInlaidEldanAugmentation((ushort)objectId);
                default:
                    log.LogWarning($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.EldanAugmentation231}!");
                    return false;
            }
        }
    }
}
