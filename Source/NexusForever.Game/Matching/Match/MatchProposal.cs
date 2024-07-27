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

        private readonly Dictionary<Faction, IMatchProposalTeam> teams = [];
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
                teams.Add(matchingQueueGroupTeam.Faction, matchingProposalTeam);
            }

            SendMatchReady(matchingQueueGroup.InProgress, Faction.Dominion);
            SendMatchReady(matchingQueueGroup.InProgress, Faction.Exile);

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
            return teams.Values;
        }

        /// <summary>
        /// Update match proposal response for <see cref="IPlayer"/>.
        /// </summary>
        public void Respond(IPlayer player, bool response)
        {
            if (!teams.TryGetValue(player.Faction1, out IMatchProposalTeam team))
                throw new InvalidOperationException();

            team.Respond(player.CharacterId, response);

            if (response == false)
            {
                Status = MatchProposalStatus.Declined;
                log.LogTrace($"Match proposal {Guid} was declined.");
            }
            else
            {
                teams.TryGetValue(Faction.Dominion, out IMatchProposalTeam t1);
                teams.TryGetValue(Faction.Dominion, out IMatchProposalTeam t2);

                if ((t1?.TeamReady ?? true) && (t2?.TeamReady ?? true))
                {
                    Status = MatchProposalStatus.Success;
                    log.LogTrace($"Match proposal {Guid} was successful.");
                }
            }

            SendMatchPendingUpdate(player.Faction1, Faction.Dominion);
            SendMatchPendingUpdate(player.Faction1, Faction.Exile);
        }

        private void SendMatchReady(bool inProgress, Faction faction)
        {
            if (!teams.TryGetValue(faction, out IMatchProposalTeam allyTeam))
                return;

            teams.TryGetValue(faction == Faction.Dominion ? Faction.Exile : Faction.Dominion, out IMatchProposalTeam enemyTeam);

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

        private void SendMatchPendingUpdate(Faction responseFaction, Faction teamFaction)
        {
            if (!teams.TryGetValue(teamFaction, out IMatchProposalTeam team))
                return;

            var test = new ServerMatchingMatchPendingUpdate()
            {
                Ally = responseFaction == teamFaction
            };

            team.Broadcast(test);
        }
    }
}
