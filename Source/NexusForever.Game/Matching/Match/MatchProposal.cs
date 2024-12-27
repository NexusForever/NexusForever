using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Matching.Match;
using NexusForever.Game.Abstract.Matching.Queue;
using NexusForever.Game.Static.Matching;
using NexusForever.Game.Static.Reputation;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Shared;
using NexusForever.Shared.Game;

namespace NexusForever.Game.Matching.Match
{
    public class MatchProposal : IMatchProposal
    {
        public Guid Guid { get; private set; }
        public MatchProposalStatus Status { get; private set; }
        public IMatchingQueueGroup MatchingQueueGroup { get; private set; }
        public IMatchingMapSelectorResult MatchingMapSelectorResult { get; private set; }

        private readonly List<IMatchProposalTeam> teams = [];
        private readonly Dictionary<ulong, IMatchProposalTeam> characterTeams = [];

        private UpdateTimer expiryTimer;

        #region Dependency Injection

        private readonly ILogger<MatchProposal> log;
        private readonly IFactory<IMatchProposalTeam> matchProposalTeamFactory;

        public MatchProposal(
            ILogger<MatchProposal> log,
            IFactory<IMatchProposalTeam> matchProposalTeamFactory)
        {
            this.log                      = log;
            this.matchProposalTeamFactory = matchProposalTeamFactory;
        }

        #endregion

        /// <summary>
        /// Initialise new match proposal with supplied <see cref="IMatchingQueueGroup"/>.
        /// </summary>
        public void Initialise(IMatchingQueueGroup matchingQueueGroup, IMatchingMapSelectorResult matchingMapSelectorResult)
        {
            if (Guid != Guid.Empty)
                throw new InvalidOperationException();

            Guid                      = Guid.NewGuid();
            MatchingQueueGroup        = matchingQueueGroup;
            MatchingMapSelectorResult = matchingMapSelectorResult;

            foreach (IMatchingQueueGroupTeam matchingQueueGroupTeam in matchingQueueGroup.GetTeams())
            {
                IMatchProposalTeam matchingProposalTeam = matchProposalTeamFactory.Resolve();
                matchingProposalTeam.Initialise(matchingQueueGroupTeam);

                teams.Add(matchingProposalTeam);
                foreach (IMatchingQueueProposalMember matchingQueueProposalMember in matchingQueueGroupTeam.GetMembers())
                    characterTeams.Add(matchingQueueProposalMember.CharacterId, matchingProposalTeam);
            }

            foreach (IMatchProposalTeam matchProposalTeam in teams)
                SendMatchReady(matchingQueueGroup.InProgress, matchProposalTeam);

            expiryTimer = new UpdateTimer(TimeSpan.FromSeconds(30d));

            log.LogTrace($"Initialised new match proposal {Guid} for matching queue group {matchingQueueGroup.Guid}.");
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public void Update(double lastTick)
        {
            if (!expiryTimer.IsTicking)
                return;

            expiryTimer.Update(lastTick);
            if (expiryTimer.HasElapsed)
                Status = MatchProposalStatus.Expired;
        }
        
        public IEnumerable<IMatchProposalTeam> GetTeams()
        {
            return teams;
        }

        private IMatchProposalTeam GetTeam(IPlayer player)
        {
            return characterTeams.TryGetValue(player.CharacterId, out IMatchProposalTeam team) ? team : null;
        }

        private IMatchProposalTeam GetOpposingTeam(IMatchProposalTeam matchProposalTeam)
        {
            return teams.SingleOrDefault(t => t.Guid != matchProposalTeam.Guid);
        }

        /// <summary>
        /// Update match proposal response for <see cref="IPlayer"/>.
        /// </summary>
        public void Respond(IPlayer player, bool response)
        {
            IMatchProposalTeam team = GetTeam(player);
            if (team == null)
                throw new InvalidOperationException();

            team.Respond(player.CharacterId, response);

            if (response == false)
            {
                Status = MatchProposalStatus.Declined;
                log.LogTrace($"Match proposal {Guid} was declined.");
            }
            else
            {
                // in debug mode it is possible for a team to have no members
                // just auto accept in this scenario
                if (teams.All(t => t.TeamReady || t.MemberCount == 0))
                {
                    Status = MatchProposalStatus.Success;
                    log.LogTrace($"Match proposal {Guid} was successful.");
                }
            }

            foreach (IMatchProposalTeam matchProposalTeam in teams)
                SendMatchPendingUpdate(team, matchProposalTeam);
        }

        private void SendMatchReady(bool inProgress, IMatchProposalTeam allyTeam)
        {
            IMatchProposalTeam enemyTeam = GetOpposingTeam(allyTeam);

            IWritable message;
            if (inProgress)
            {
                message = new ServerMatchingMatchInProgressReady()
                {
                    MatchType    = MatchingQueueGroup.MatchType,
                    TotalAllies  = allyTeam.MemberCount,
                    TotalEnemies = enemyTeam?.MemberCount ?? 0u,
                };
            }
            else
            {
                message = new ServerMatchingMatchReady()
                {
                    MatchType    = MatchingQueueGroup.MatchType,
                    TotalAllies  = allyTeam.MemberCount,
                    TotalEnemies = enemyTeam?.MemberCount ?? 0u,
                };
            }

            allyTeam.Broadcast(message);
        }

        private void SendMatchPendingUpdate(IMatchProposalTeam responseTeam, IMatchProposalTeam team)
        {
            var test = new ServerMatchingMatchPendingUpdate()
            {
                Ally = responseTeam.Guid == team.Guid
            };

            team.Broadcast(test);
        }
    }
}
