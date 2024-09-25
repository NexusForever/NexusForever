using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
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
        private readonly Dictionary<Guid, IMatch> matches = [];

        private readonly Dictionary<ulong, IMatchCharacter> characters = [];

        #region Dependency Injection

        private readonly ILogger<MatchManager> log;

        private readonly IFactory<IMatchCharacter> matchCharacterFactory;
        private readonly IFactory<IMatchProposal> matchProposalFactory;
        private readonly IMatchFactory matchFactory;
        private readonly IMatchingManager matchingManager;
        private readonly IMatchingQueueTimeManager matchingQueueTimeManager;

        public MatchManager(
            ILogger<MatchManager> log,
            IFactory<IMatchCharacter> matchCharacterFactory,
            IFactory<IMatchProposal> matchProposalFactory,
            IMatchFactory matchFactory,
            IMatchingManager matchingManager,
            IMatchingQueueTimeManager matchingQueueTimeManager)
        {
            this.log                      = log;

            this.matchCharacterFactory    = matchCharacterFactory;
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
                            GetMatchCharacter(matchProposalTeamMember.MatchingQueueProposalMember.CharacterId).RemoveMatchProposal();

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
                    match.Update(lastTick);

                    switch (match.Status)
                    {
                        case MatchStatus.Finalised:
                        {
                            matchesToRemove.Add(match);
                            break;
                        }
                    }
                }

                foreach (IMatch match in matchesToRemove)
                {
                    matches.Remove(match.Guid);
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

            IMatch match = matchFactory.CreateMatch(matchProposal.MatchingQueueGroup.MatchType);
            match.Initialise(matchProposal);
            matches.Add(match.Guid, match);

            // find unique set of matching queue proposals to remove
            // this is required since multiple characters can be in the same matching queue proposal
            var matchingQueueProposalsToRemove = new HashSet<IMatchingQueueProposal>();
            foreach (IMatchProposalTeam matchProposalTeam in matchProposal.GetTeams())
                foreach (IMatchProposalTeamMember matchProposalTeamMember in matchProposalTeam.GetMembers())
                    matchingQueueProposalsToRemove.Add(matchProposalTeamMember.MatchingQueueProposalMember.MatchingQueueProposal);

            foreach (IMatchingQueueProposal matchingQueueProposal in matchingQueueProposalsToRemove)
                matchProposal.MatchingQueueGroup.RemoveMatchingQueueProposal(matchingQueueProposal, null);
        }

        /// <summary>
        /// Invoked when <see cref="IPlayer"/> logs in.
        /// </summary>
        public void OnLogin(IPlayer player)
        {
            // update client with match information
            IMatchCharacter matchCharacter = GetMatchCharacter(player.CharacterId);
            matchCharacter.Match?.OnLogin(player);
        }

        /// <summary>
        /// Invoked when <see cref="IPlayer"/> logs out.
        /// </summary>
        public void OnLogout(IPlayer player)
        {
            // decline match proposal on logout
            IMatchCharacter matchCharacter = GetMatchCharacter(player.CharacterId);
            matchCharacter.MatchProposal?.Respond(player, false);
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
                    GetMatchCharacter(matchProposalTeamMember.MatchingQueueProposalMember.CharacterId).AddMatchProposal(matchProposal);
                    //characterMatchProposals.Add(matchProposalTeamMember.MatchingQueueProposalMember.CharacterId, matchProposal);

                    IMatchingCharacter matchingCharacter = matchingManager.GetMatchingCharacter(matchProposalTeamMember.MatchingQueueProposalMember.CharacterId);
                    foreach (IMatchingCharacterQueue matchingCharacterQueue in matchingCharacter.GetMatchingCharacterQueues())
                        matchingQueueGroupsToPause.Add(matchingCharacterQueue.MatchingQueueGroup);
                }
            }

            foreach (IMatchingQueueGroup matchingQueueGroupToPause in matchingQueueGroupsToPause)
                matchingQueueGroupToPause.IsPaused = true;
        }

        /// <summary>
        /// Return <see cref="IMatchCharacter"/> for supplied character id.
        /// </summary>
        /// <remarks>
        /// Will return a new <see cref="IMatchCharacter"/> if one does not exist.
        /// </remarks>
        public IMatchCharacter GetMatchCharacter(ulong characterId)
        {
            if (!characters.TryGetValue(characterId, out IMatchCharacter characterInfo))
            {
                characterInfo = matchCharacterFactory.Resolve();
                characterInfo.Initialise(characterId);
                characters.Add(characterId, characterInfo);
            }

            return characterInfo;
        }
    }
}
