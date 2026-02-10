using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Prerequisite;

namespace NexusForever.Game.Prerequisite.Check
{
    [PrerequisiteCheck(PrerequisiteType.PetFlair)]
    public class PrerequisiteCheckPetFlair : BasePrerequisiteHandler, IPrerequisiteCheck
    {
        #region Dependency Injection

        public PrerequisiteCheckPetFlair(
            ILogger<BasePrerequisiteHandler> log)
            : base(log)
        {
        }

        #endregion

        public bool Meets(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            IPetCustomisation customisation = player.PetCustomisationManager.GetCustomisation(PetType.HoverBoard, objectId);
            bool hasCustomisation = customisation != null;
            
            return MatchBoolean(hasCustomisation, comparison, PrerequisiteType.PetFlair);
        }
    }
}
