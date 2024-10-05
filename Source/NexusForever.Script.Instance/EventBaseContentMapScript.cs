using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Event;
using NexusForever.Game.Abstract.Map.Instance;
using NexusForever.Game.Static.Event;
using NexusForever.Script.Template;

namespace NexusForever.Script.Instance
{
    public abstract class EventBaseContentMapScript : IContentMapScript, IOwnedScript<IContentMapInstance>
    {
        public abstract uint PublicEventId { get; }

        protected IContentMapInstance map;
        protected IPublicEvent publicEvent;

        /// <summary>
        /// Invoked when <see cref="IContentMapInstance"/> is loaded.
        /// </summary>
        public void OnLoad(IContentMapInstance owner)
        {
            map         = owner;
            publicEvent = map.PublicEventManager.CreateEvent(PublicEventId);
        }

        /// <summary>
        /// Invoked when <see cref="IGridEntity"/> is added to map.
        /// </summary>
        public virtual void OnAddToMap(IGridEntity entity)
        {
            if (entity is not IPlayer player)
                return;

            publicEvent.JoinEvent(player, PublicEventTeam.PublicTeam);
        }

        /// <summary>
        /// Invoked when <see cref="IPublicEvent"/> finishes with the winning <see cref="IPublicEventTeam"/>.
        /// </summary>
        public void OnPublicEventFinish(IPublicEvent publicEvent, IPublicEventTeam publicEventTeam)
        {
            if (this.publicEvent != publicEvent)
                return;

            map.Match?.MatchFinish();
        }

        /// <summary>
        /// Invoked when the <see cref="IMatch"/> for the map finishes.
        /// </summary>
        public void OnMatchFinish()
        {
            publicEvent.Finish(PublicEventTeam.PublicTeam);
        }
    }
}
