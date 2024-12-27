using NexusForever.Game.Abstract.Cinematic;
using NexusForever.Game.Abstract.Cinematic.Cinematics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Alizar.EverstarGrove
{
    [ScriptFilterOwnerId(990)]
    public class EverstarGroveMapScript : IMapScript, IOwnedScript<IBaseMap>
    {
        public enum Quest : ushort
        {
            NaturesUprising = 6296
        }

        #region Dependency Injection

        private readonly ICinematicFactory cinematicFactory;

        public EverstarGroveMapScript(
            ICinematicFactory cinematicFactory)
        {
            this.cinematicFactory = cinematicFactory;
        }

        #endregion

        public void OnAddToMap(IGridEntity entity)
        {
            if (entity is not IPlayer player)
                return;

            if (player.QuestManager.GetQuestState(Quest.NaturesUprising) == null)
                player.CinematicManager.QueueCinematic(cinematicFactory.CreateCinematic<IEverstarGroveOnCreate>());
        }
    }
}
