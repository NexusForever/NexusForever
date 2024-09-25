using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Event;
using NexusForever.Game.Abstract.Map.Instance;
using NexusForever.Game.Abstract.Matching.Match;
using NexusForever.Game.Static.Event;
using NexusForever.Game.Static.Matching;
using NexusForever.Script.Template;

namespace NexusForever.Script.Instance
{
    public abstract class EventBasePvpContentMapScript : IContentPvpMapScript, IOwnedScript<IContentPvpMapInstance>
    {
        public abstract uint PublicEventId { get; }
        public abstract uint PublicSubEventId { get; }

        protected IContentPvpMapInstance map;
        protected IPublicEvent publicEvent;
        protected IPublicEvent publicSubEvent;

        /// <summary>
        /// Invoked when <see cref="IContentPvpMapInstance"/> is loaded.
        /// </summary>
        public void OnLoad(IContentPvpMapInstance owner)
        {
            map            = owner;
            publicEvent    = map.PublicEventManager.CreateEvent(PublicEventId);
            publicSubEvent = map.PublicEventManager.CreateEvent(PublicSubEventId);
        }

        /// <summary>
        /// Invoked when <see cref="IGridEntity"/> is added to map.
        /// </summary>
        public void OnAddToMap(IGridEntity entity)
        {
            if (entity is not IPlayer player)
                return;

            if (map.Match == null)
                return;

            IMatchTeam team = map.Match.GetTeam(player.CharacterId);
            if (team == null)
                return;

            publicEvent.JoinEvent(player, team.Team == MatchTeam.Red ? PublicEventTeam.RedTeam_2 : PublicEventTeam.BlueTeam_2);
            publicSubEvent.JoinEvent(player, PublicEventTeam.PublicTeam);
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnPublicEventFinish(IPublicEvent publicEvent, IPublicEventTeam publicEventTeam)
        {
            if (this.publicEvent != publicEvent)
                return;

            if (map.Match is not IPvpMatch pvpMatch)
                return;

            MatchWinner winner;
            if (publicEventTeam == null)
                winner = MatchWinner.Draw;
            else
                winner = publicEventTeam.Team == PublicEventTeam.RedTeam_2 ? MatchWinner.Red : MatchWinner.Blue;

            pvpMatch.MatchFinish(winner, MatchEndReason.Completed);
        }

        /// <summary>
        /// Invoked when the <see cref="IPvpMatch"/> for the map changes <see cref="PvpGameState"/>.
        /// </summary>
        public void OnPvpMatchState(PvpGameState state)
        {
            publicEvent.OnPvpMatchState(state);
            publicSubEvent.OnPvpMatchState(state);
        }

        /// <summary>
        /// Invoked when the <see cref="IPvpMatch"/> for the map finishes.
        /// </summary>
        public virtual void OnPvpMatchFinish(MatchWinner matchWinner, MatchEndReason matchEndReason)
        {
            publicEvent.Finish(matchWinner == MatchWinner.Red ? PublicEventTeam.RedTeam_2 : PublicEventTeam.BlueTeam_2);
            publicSubEvent.Finish(PublicEventTeam.PublicTeam);
        }
    }
}
