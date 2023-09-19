using NexusForever.Game.Abstract.Cinematic.Cinematics;
using NexusForever.Game.Abstract.Cinematic;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Script.Template.Filter;
using NexusForever.Script.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusForever.Script.Olyssia.LevianBay
{
    [ScriptFilterOwnerId(1387)]
    public class LevianBayMapScript : IMapScript, IOwnedScript<IBaseMap>
    {
        public enum Quest : ushort
        {
            LightingTheWay = 6780
        }

        #region Dependency Injection

        private readonly ICinematicFactory cinematicFactory;

        public LevianBayMapScript(
            ICinematicFactory cinematicFactory)
        {
            this.cinematicFactory = cinematicFactory;
        }

        #endregion

        public void OnAddToMap(IGridEntity entity)
        {
            if (entity is not IPlayer player)
                return;

            if (player.QuestManager.GetQuestState(Quest.LightingTheWay) == null)
            {
                player.CinematicManager.QueueCinematic(cinematicFactory.CreateCinematic<ILevianBayOnCreate>());
            }
        }
    }
}
