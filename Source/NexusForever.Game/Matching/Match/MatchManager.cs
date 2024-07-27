using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Matching.Match;
using NexusForever.Game.Abstract.Matching.Queue;
using NexusForever.Game.Static.Matching;
using NexusForever.Network.World.Message.Model;
using NexusForever.Shared;

namespace NexusForever.Game.Matching.Match
{
    public class MatchManager : IMatchManager
    {
        private readonly HashSet<IMatchProposal> matchProposals = [];
        private readonly Dictionary<ulong, IMatchProposal> characterMatchProposals = [];

        private readonly Dictionary<Guid, IMatch> matches = [];
        private readonly Dictionary<ulong, IMatch> characterMatches = [];

        #region Dependency Injection

        private readonly ILogger<MatchManager> log;

        private readonly IFactory<IMatchProposal> matchProposalFactory;
        private readonly IFactory<IMatch> matchFactory;
        private readonly IMatchingManager matchingManager;
        private readonly IMatchingQueueTimeManager matchingQueueTimeManager;

        public MatchManager(
            ILogger<MatchManager> log,
            IFactory<IMatchProposal> matchProposalFactory,
            IFactory<IMatch> matchFactory,
            IMatchingManager matchingManager,
            IMatchingQueueTimeManager matchingQueueTimeManager)
        {
            this.log                      = log;

            this.matchProposalFactory     = matchProposalFactory;
            this.matchFactory             = matchFactory;
            this.matchingManager          = matchingManager;
            this.matchingQueueTimeManager = matchingQueueTimeManager;
        }

        #endregion

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public void Update(double lastTick)
        {
            UpdateMatchProposals(lastTick);
            UpdateMatches(lastTick);
        }

        private void UpdateMatchProposals(double lastTick)
        {
            if (matchProposals.Count > 0)
            {
                var matchProposalsToRemove = new List<IMatchProposal>();
                foreach (IMatchProposal matchProposal in matchProposals)
                {
                    matchProposal.Update(lastTick);
                    switch (matchProposal.Status)
                    {
                        case MatchProposalStatus.Declined:
                        case MatchProposalStatus.Expired:
                        {
                            MatchProposalDeclined(matchProposal);
                            matchProposalsToRemove.Add(matchProposal);
                            break;
                        }
                        case MatchProposalStatus.Success:
                        {
                            CreateMatch(matchProposal);
                            matchProposalsToRemove.Add(matchProposal);
                            break;
                        }
                    }
                }

                foreach (IMatchProposal matchProposal in matchProposalsToRemove)
                {
                    matchProposals.Remove(matchProposal);
                    foreach (IMatchProposalTeam matchProposalTeam in matchProposal.GetTeams())
                        foreach (IMatchProposalTeamMember matchProposalTeamMember in matchProposalTeam.GetMembers())
                            characterMatchProposals.Remove(matchProposalTeamMember.MatchingQueueProposalMember.CharacterId);

                    log.LogTrace($"MatchProposal {matchProposal.Guid} removed from store.");
                }
            }
        }

        private void UpdateMatches(double lastTick)
        {
            if (matches.Count > 0)
            {
                var matchesToRemove = new List<IMatch>();
                foreach (IMatch match in matches.Values)
                {
                    switch (match.Status)
                    {
                        case MatchStatus.Complete:
                        {
                            matchesToRemove.Add(match);
                            break;
                        }
                    }
                }

                foreach (IMatch match in matchesToRemove)
                {
                    matches.Remove(match.Guid);
                    foreach (IMatchTeam matchTeam in match.GetTeams())
                        foreach (IMatchTeamMember matchTeamMember in matchTeam.GetMembers())
                            characterMatches.Remove(matchTeamMember.CharacterId);

                    log.LogTrace($"Match {match.Guid} removed from store.");
                }
            }
        }

        private void MatchProposalDeclined(IMatchProposal matchProposal)
        {
            // find unique set of matching queue proposals which responded with decline
            // this is required since multiple characters can be in the same matching queue proposal
            var matchingQueueProposalsToRemove = new HashSet<IMatchingQueueProposal>();
            foreach (IMatchProposalTeam matchProposalTeam in matchProposal.GetTeams())
                foreach (IMatchProposalTeamMember matchProposalTeamMember in matchProposalTeam.GetMembers())
                    if (matchProposalTeamMember.Response == false)
                        matchingQueueProposalsToRemove.Add(matchProposalTeamMember.MatchingQueueProposalMember.MatchingQueueProposal);

            foreach (IMatchingQueueProposal matchingQueueProposal in matchingQueueProposalsToRemove)
                matchProposal.MatchingQueueGroup.RemoveMatchingQueueProposal(matchingQueueProposal, MatchingQueueResult.Declined);

            // any remaining matching queue proposals should be unpaused
            matchProposal.MatchingQueueGroup.IsPaused = false;

            foreach (IMatchProposalTeam matchProposalTeam in matchProposal.GetTeams())
                matchProposalTeam.Broadcast(new ServerMatchingMatchReadyCancel());
        }

        private void CreateMatch(IMatchProposal matchProposal)
        {
            if (!matchProposal.MatchingQueueGroup.IsSolo)
                matchingQueueTimeManager.Update(matchProposal.MatchingQueueGroup);

            // TODO: create party

            IMatch match = matchFactory.Resolve();
            match.Initialise(matchProposal);
            matches.Add(match.Guid, match);

            // find unique set of matching queue proposals to remove
            // this is required since multiple characters can be in the same matching queue proposal
            var matchingQueueProposalsToRemove = new HashSet<IMatchingQueueProposal>();
            foreach (IMatchProposalTeam matchProposalTeam in matchProposal.GetTeams())
            {
                foreach (IMatchProposalTeamMember matchProposalTeamMember in matchProposalTeam.GetMembers())
                {
                    characterMatches.Add(matchProposalTeamMember.MatchingQueueProposalMember.CharacterId, match);
                    matchingQueueProposalsToRemove.Add(matchProposalTeamMember.MatchingQueueProposalMember.MatchingQueueProposal);
                }
            }

            foreach (IMatchingQueueProposal matchingQueueProposal in matchingQueueProposalsToRemove)
                matchProposal.MatchingQueueGroup.RemoveMatchingQueueProposal(matchingQueueProposal, null);
        }

        /// <summary>
        /// Create a new <see cref="IMatchProposal"/> for the supplied <see cref="IMatchingQueueGroup"/>.
        /// </summary>
        public void CreateMatchProposal(IMatchingQueueGroup matchingQueueGroup, IMatchingMapSelectorResult matchingMapSelectorResult)
        {
            IMatchProposal matchProposal = matchProposalFactory.Resolve();
            matchProposal.Initialise(matchingQueueGroup, matchingMapSelectorResult);
            matchProposals.Add(matchProposal);

            // find unique set of matching queue groups to pause
            // this is required since multiple characters can be in the same matching queue proposal
            var matchingQueueGroupsToPause = new HashSet<IMatchingQueueGroup>();
            foreach (IMatchProposalTeam matchProposalTeam in matchProposal.GetTeams())
            {
                foreach (IMatchProposalTeamMember matchProposalTeamMember in matchProposalTeam.GetMembers())
                {
                    characterMatchProposals.Add(matchProposalTeamMember.MatchingQueueProposalMember.CharacterId, matchProposal);

                    IMatchingCharacter ss = matchingManager.GetMatchingCharacter(matchProposalTeamMember.MatchingQueueProposalMember.CharacterId);
                    foreach (IMatchingCharacterQueue matchingCharacterQueue in ss.GetMatchingCharacterQueues())
                        matchingQueueGroupsToPause.Add(matchingCharacterQueue.MatchingQueueGroup);
                }
            }

            foreach (IMatchingQueueGroup matchingQueueGroupToPause in matchingQueueGroupsToPause)
                matchingQueueGroupToPause.IsPaused = true;
        }

        /// <summary>
        /// Return <see cref="IMatchProposal"/> the supplied character is currently responding to.
        /// </summary>
        public IMatchProposal GetMatchProposal(ulong characterId)
        {
            return characterMatchProposals.TryGetValue(characterId, out IMatchProposal matchProposal) ? matchProposal : null;
        }

        /// <summary>
        /// Return <see cref="IMatch"/> the supplied character is currently in.
        /// </summary>
        public IMatch GetMatch(ulong characterId)
        {
            return characterMatches.TryGetValue(characterId, out IMatch match) ? match : null;
        }
    }
}
