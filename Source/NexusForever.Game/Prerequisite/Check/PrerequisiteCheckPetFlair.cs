using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Prerequisite;

namespace NexusForever.Game.Prerequisite.Check
{
    [PrerequisiteCheck(PrerequisiteType.PetFlair)]
    public class PrerequisiteCheckPetFlair : IPrerequisiteCheck
    {
        #region Dependency Injection

        private readonly ILogger<PrerequisiteCheckPetFlair> log;

        public PrerequisiteCheckPetFlair(
            ILogger<PrerequisiteCheckPetFlair> log)
        {
            this.log = log;
        }

        #endregion

        public bool Meets(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            switch (comparison)
            {
                case PrerequisiteComparison.Equal:
                    return player.PetCustomisationManager.GetCustomisation(PetType.HoverBoard, objectId) != null;
                case PrerequisiteComparison.NotEqual:
                    return player.PetCustomisationManager.GetCustomisation(PetType.HoverBoard, objectId) == null;
                default:
                    log.LogWarning($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.PetFlair}!");
                    return false;
            }
        }
    }
}
