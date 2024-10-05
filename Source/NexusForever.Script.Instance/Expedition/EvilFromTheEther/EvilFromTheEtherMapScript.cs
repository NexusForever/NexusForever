using NexusForever.Game.Abstract.Cinematic;
using NexusForever.Game.Abstract.Cinematic.Cinematics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Instance.Expedition.EvilFromTheEther
{
    [ScriptFilterOwnerId(3404)]
    public class EvilFromTheEtherMapScript : EventBaseContentMapScript
    {
        public override uint PublicEventId => 781u;

        #region Dependency Injection

        private readonly ICinematicFactory cinematicFactory;

        public EvilFromTheEtherMapScript(
            ICinematicFactory cinematicFactory)
        {
            this.cinematicFactory = cinematicFactory;
        }

        #endregion

        /// <summary>
        /// Invoked when <see cref="IGridEntity"/> is added to map.
        /// </summary>
        public override void OnAddToMap(IGridEntity entity)
        {
            base.OnAddToMap(entity);

            if (entity is not IPlayer player)
                return;

            player.CinematicManager.QueueCinematic(cinematicFactory.CreateCinematic<IEvilFromTheEtherOnCreate>());
        }
    }
}
