using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Static.Prerequisite;

namespace NexusForever.Game.Prerequisite.Check
{
    /// <summary>
    /// Is the <see cref="PrerequisiteEntry"/> object id an Eldan augmentation selected in the active spec?
    /// </summary>
    [PrerequisiteCheck(PrerequisiteType.EldanAugmentation227)]
    public class PrerequisiteCheckEldanAugmentation227 : IPrerequisiteCheck
    {
        #region Dependency Injection

        private readonly ILogger<PrerequisiteCheckEldanAugmentation227> log;

        public PrerequisiteCheckEldanAugmentation227(
            ILogger<PrerequisiteCheckEldanAugmentation227> log)
        {
            this.log = log;
        }

        #endregion

        public bool Meets(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            IActionSet actionSet = player.SpellManager.GetActionSet(player.SpellManager.ActiveActionSet);
            bool selected = actionSet.GetAmp((ushort)objectId) != null;

            switch (comparison)
            {
                case PrerequisiteComparison.Equal:
                    return selected;
                case PrerequisiteComparison.NotEqual:
                    return !selected;
                default:
                    log.LogWarning($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.EldanAugmentation227}!");
                    return false;
            }
        }
    }
}
