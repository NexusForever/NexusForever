using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Abstract.Matching;
using NexusForever.Game.Abstract.Matching.Match;
using NexusForever.Game.Abstract.Matching.Queue;
using NexusForever.Game.Static.Matching;
using NexusForever.Game.Static.Reputation;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Shared;

namespace NexusForever.Game.Matching.Match
{
    public class Match : IMatch
    {
        public Guid Guid { get; private set; }
        public MatchStatus Status { get; set; }
        public IMatchingMap MatchingMap { get; private set; }

        private readonly Dictionary<Faction, IMatchTeam> teams = [];

        #region Dependency Injection

        private readonly IMatchingDataManager matchingDataManager;
        private readonly IFactory<IMatchTeam> matchTeamFactory;

        public Match(
            IMatchingDataManager matchingDataManager,
            IFactory<IMatchTeam> matchTeamFactory)
        {
            this.matchingDataManager = matchingDataManager;
            this.matchTeamFactory    = matchTeamFactory;
        }

        #endregion

        /// <summary>
        /// Initialise the match with the supplied <see cref="IMatchProposal"/>
        /// </summary>
        public void Initialise(IMatchProposal matchProposal)
        {
            if (Guid != Guid.Empty)
                throw new InvalidOperationException();

            Guid        = Guid.NewGuid();
            MatchingMap = matchProposal.MatchingMapSelectorResult.MatchingMap;
            
            byte team = 0;
            foreach (IMatchingQueueGroupTeam matchingQueueGroupTeam in matchProposal.MatchingQueueGroup.GetTeams())
            {
                IMapEntrance mapEntrance = matchingDataManager.GetMapEntrance(MatchingMap.GameMapEntry.WorldId, team++);

                IMatchTeam matchTeam = matchTeamFactory.Resolve();
                matchTeam.Initialise(matchingQueueGroupTeam, mapEntrance);
                teams.Add(matchingQueueGroupTeam.Faction, matchTeam);
            }

            Broadcast(new ServerMatchingMatchJoined
            {
                MatchingGameMap = MatchingMap.Id
            });
        }

        /// <summary>
        /// Return <see cref="IMatchTeam"/> for supplied <see cref="Faction"/>.
        /// </summary>
        private IMatchTeam GetTeam(Faction faction)
        {
            return teams.TryGetValue(faction, out IMatchTeam team) ? team : null;
        }

        public IEnumerable<IMatchTeam> GetTeams()
        {
            return teams.Values;
        }

        /// <summary>
        /// Invoked when <see cref="IPlayer"/> enters the match.
        /// </summary>
        public void MatchEnter(IPlayer player)
        {
            IMatchTeam team = GetTeam(player.Faction1);
            team.MatchEnter(player.CharacterId, MatchingMap);
        }

        /// <summary>
        /// Invoked when <see cref="IPlayer"/> exits the match.
        /// </summary>
        public void MatchExit(IPlayer player)
        {
            IMatchTeam team = GetTeam(player.Faction1);
            team.MatchExit(player.CharacterId);

            // TODO: probably want to remove this once parties are added
            // this really just prevents the match from never ending once all players have left
            if (GetTeams()
                .SelectMany(t => t.GetMembers())
                .All(m => !m.InMatch))
            {
                Status = MatchStatus.Complete;
            }
        }

        /// <summary>
        /// Get return <see cref="IMapPosition"/> for <see cref="IPlayer"/>.
        /// </summary>
        /// <remarks>
        /// Return position is the position of the player before entering the match.
        /// </remarks>
        public IMapPosition GetReturnPosition(IPlayer player)
        {
            IMatchTeam team = GetTeam(player.Faction1);
            return team.GetReturnPosition(player.CharacterId);
        }

        private void Broadcast(IWritable message)
        {
            foreach (IMatchTeam party in teams.Values)
                party.Broadcast(message);
        }
    }
}
