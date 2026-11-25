using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Prerequisite;

namespace NexusForever.Game.Prerequisite.Check
{
    [PrerequisiteCheck(PrerequisiteType.ItemProficiency)]
    public class PrerequisiteCheckItemProficiency : IPrerequisiteCheck
    {
        #region Dependency Injection

        private readonly ILogger<PrerequisiteCheckItemProficiency> log;

        public PrerequisiteCheckItemProficiency(
            ILogger<PrerequisiteCheckItemProficiency> log)
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
                    return (player.GetItemProficiencies() & (ItemProficiency)value) == 0;
                case PrerequisiteComparison.Equal:
                    return (player.GetItemProficiencies() & (ItemProficiency)value) != 0;
                default:
                    log.LogWarning($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.ItemProficiency}!");
                    return false;
            }
        }
    }
}
