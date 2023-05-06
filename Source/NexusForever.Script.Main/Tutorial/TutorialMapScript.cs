using NexusForever.Game.Abstract.Cinematic.Cinematics;
using NexusForever.Game.Abstract.Cinematic;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Main.Tutorial
{
    [ScriptFilterOwnerId(3460)]
    public class TutorialMapScript : IMapScript, IOwnedScript<IBaseMap>
    {
        #region Dependency Injection

        private readonly ICinematicFactory cinematicFactory;

        public TutorialMapScript(
            ICinematicFactory cinematicFactory)
        {
            this.cinematicFactory = cinematicFactory;
        }

        #endregion

        public void OnAddToMap(IGridEntity entity)
        {
            if (entity is not IPlayer player)
                return;

            player.CinematicManager.QueueCinematic(cinematicFactory.CreateCinematic<INoviceTutorialOnEnter>());
        }
    }
}
