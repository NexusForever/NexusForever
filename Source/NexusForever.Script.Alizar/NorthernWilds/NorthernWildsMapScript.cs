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

namespace NexusForever.Script.Alizar.NorthernWilds
{
    [ScriptFilterOwnerId(426)]
    public class NorthernWildsMapScript : IMapScript, IOwnedScript<IBaseMap>
    {
        public enum Quest : ushort
        {
            ReportingForDuty = 3480
        }

        #region Dependency Injection

        private readonly ICinematicFactory cinematicFactory;

        public NorthernWildsMapScript(
            ICinematicFactory cinematicFactory)
        {
            this.cinematicFactory = cinematicFactory;
        }

        #endregion

        public void OnAddToMap(IGridEntity entity)
        {
            if (entity is not IPlayer player)
                return;

            if (player.QuestManager.GetQuestState(Quest.ReportingForDuty) == null)
            {
                player.CinematicManager.QueueCinematic(cinematicFactory.CreateCinematic<INorthernWildsOnCreate>());
            }
        }

        // TODO: OnEnterZone work for empowered tower once map script base is ready
    }
}
