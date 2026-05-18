using NexusForever.Game.Abstract.Cinematic;
using NexusForever.Game.Abstract.Cinematic.Cinematics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Olyssia.CrimsonIsle
{
    [ScriptFilterOwnerId(870)]
    public class CrimsonIsleMapScript : IMapScript, IOwnedScript<IBaseMap>
    {
        public enum Quest : ushort
        {
            MindTheMinesScrapTheScrab = 5593
        }

        public enum Zones : ushort
        {
            CrashSite = 1611
        }

        #region Dependency Injection

        private readonly ICinematicFactory cinematicFactory;

        public CrimsonIsleMapScript(
            ICinematicFactory cinematicFactory)
        {
            this.cinematicFactory = cinematicFactory;
        }

        #endregion

        public void OnAddToMap(IGridEntity entity)
        {
            if (entity is not IPlayer player)
                return;

            if (player.QuestManager.GetQuestState(Quest.MindTheMinesScrapTheScrab) == null)
            {
                player.CinematicManager.QueueCinematic(cinematicFactory.CreateCinematic<ICrimsonIsleOnCreate>());
            }
        }

        // TODO: OnEnterZone for Crash Site objective
    }
}
