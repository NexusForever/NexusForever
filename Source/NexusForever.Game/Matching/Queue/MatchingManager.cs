using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Matching;
using NexusForever.Game.Abstract.Matching.Queue;
using NexusForever.Game.Map.Search;
using NexusForever.Game.Static.Matching;
using NexusForever.Game.Static.Reputation;
using NexusForever.Network.World.Message.Model;
using NexusForever.Shared;

namespace NexusForever.Game.Matching.Queue
{
    public class MatchingManager : IMatchingManager
    {
        private readonly Dictionary<Faction, IMatchingQueueManager> pveMatchingQueueManagers = [];
        private IMatchingQueueManager pvpMatchingQueueManager;

        private readonly ConcurrentQueue<IMatchingQueueProposal> incomingMatchingQueueProposals = [];

        private readonly Dictionary<ulong, IMatchingCharacter> characters = [];

        private readonly List<IMatchingRoleCheck> matchingRoleChecks = [];
        private readonly Dictionary<ulong, IMatchingRoleCheck> characterMatchingRoleChecks = [];

        #region Dependency Injection

        private readonly ILogger<MatchingManager> log;

        private readonly IMatchingDataManager matchingDataManager;
        private readonly IMatchingQueueTimeManager matchingQueueTimeManager;
        private readonly IFactory<IMatchingQueueManager> matchingQueueManagerFactory;
        private readonly IFactory<IMatchingQueueProposal> matchingQueueProposalFactory;
        private readonly IFactory<IMatchingRoleCheck> matchingRoleCheckFactory;
        private readonly IFactory<IMatchingCharacter> matchingCharacterFactory;

        public MatchingManager(
            ILogger<MatchingManager> log,
            IMatchingDataManager matchingDataManager,
            IMatchingQueueTimeManager matchingQueueTimeManager,
            IFactory<IMatchingQueueManager> matchingQueueManagerFactory,
            IFactory<IMatchingQueueProposal> matchingQueueProposalFactory,
            IFactory<IMatchingRoleCheck> matchingRoleCheckFactory,
            IFactory<IMatchingCharacter> matchingCharacterFactory)
        {
            this.log                          = log;
            this.matchingDataManager          = matchingDataManager;
            this.matchingQueueTimeManager     = matchingQueueTimeManager;
            this.matchingQueueManagerFactory  = matchingQueueManagerFactory;
            this.matchingQueueProposalFactory = matchingQueueProposalFactory;
            this.matchingRoleCheckFactory     = matchingRoleCheckFactory;
            this.matchingCharacterFactory     = matchingCharacterFactory;
        }

        #endregion

        /// <summary>
        /// Initialise data and queue managers for <see cref="IMatchingManager"/>.
        /// </summary>
        public void Initialise()
        {
            if (pveMatchingQueueManagers.Count > 0)
                throw new InvalidOperationException();

            matchingDataManager.Initialise();
            matchingQueueTimeManager.Initialise();

            InitialisePvEQueueManagers();
            InitialisePvPQueueManagers();
        }

        private void InitialisePvEQueueManagers()
        {
            void CreatePvEQueueManager(Faction faction)
            {
                IMatchingQueueManager pveMatchingQueueManager = matchingQueueManagerFactory.Resolve();
                pveMatchingQueueManager.Initialise();
                pveMatchingQueueManagers.Add(faction, pveMatchingQueueManager);
            }

            CreatePvEQueueManager(Faction.Dominion);
            CreatePvEQueueManager(Faction.Exile);
        }

        private void InitialisePvPQueueManagers()
        {
            pvpMatchingQueueManager = matchingQueueManagerFactory.Resolve();
            pvpMatchingQueueManager.Initialise();
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public void Update(double lastTick)
        {
            foreach (IMatchingQueueManager matchingQueueManager in pveMatchingQueueManagers.Values)
                matchingQueueManager.Update(lastTick);

            pvpMatchingQueueManager.Update(lastTick);

            UpdateIncomingProposals();
            UpdateRoleChecks(lastTick);
        }

        private void UpdateIncomingProposals()
        {
            if (!incomingMatchingQueueProposals.TryDequeue(out IMatchingQueueProposal matchingQueueProposal))
                return;

            JoinQueue(matchingQueueProposal);
        }

        private void UpdateRoleChecks(double lastTick)
        {
            if (matchingRoleChecks.Count > 0)
            {
                var matchingRoleChecksToRemove = new List<IMatchingRoleCheck>();
                foreach (IMatchingRoleCheck matchingRoleCheck in matchingRoleChecks)
                {
                    matchingRoleCheck.Update(lastTick);
                    switch (matchingRoleCheck.Status)
                    {
                        case MatchingRoleCheckStatus.Declined:
                        {
                            log.LogTrace($"Role check {matchingRoleCheck.Guid} was declined.");
                            matchingRoleChecksToRemove.Add(matchingRoleCheck);
                            break;
                        }
                        case MatchingRoleCheckStatus.Expired:
                        {
                            log.LogTrace($"Role check {matchingRoleCheck.Guid} has expired.");
                            matchingRoleChecksToRemove.Add(matchingRoleCheck);
                            break;
                        }
                        case MatchingRoleCheckStatus.Success:
                        {
                            MatchingRoleCheckSuccessful(matchingRoleCheck);
                            matchingRoleChecksToRemove.Add(matchingRoleCheck);
                            break;
                        }
                    }
                }

                foreach (IMatchingRoleCheck matchingRoleCheck in matchingRoleChecksToRemove)
                {
                    matchingRoleChecks.Remove(matchingRoleCheck);
                    foreach (IMatchingRoleCheckMember matchingRoleCheckMember in matchingRoleCheck.GetMembers())
                        characterMatchingRoleChecks.Remove(matchingRoleCheckMember.CharacterId);

                    log.LogTrace($"Role check {matchingRoleCheck.Guid} removed from store.");
                }
            }
        }

        private void MatchingRoleCheckSuccessful(IMatchingRoleCheck matchingRoleCheck)
        {
            foreach (IMatchingRoleCheckMember matchingRoleCheckMember in matchingRoleCheck.GetMembers())
            {
                matchingRoleCheck.MatchingQueueProposal.AddMember(matchingRoleCheckMember.CharacterId, matchingRoleCheckMember.Roles.Value);

                // remove member from all solo queues
                foreach (IMatchingCharacterQueue matchingCharacterQueue in GetMatchingCharacter(matchingRoleCheckMember.CharacterId).GetMatchingCharacterQueues())
                    if (!matchingCharacterQueue.MatchingQueueProposal.IsParty)
                        matchingCharacterQueue.MatchingQueueGroup.RemoveMatchingQueueProposal(matchingCharacterQueue.MatchingQueueProposal);
            }

            log.LogTrace($"Role check {matchingRoleCheck.Guid} was successful, adding match proposal {matchingRoleCheck.MatchingQueueProposal.Guid} to queue.");
            JoinQueue(matchingRoleCheck.MatchingQueueProposal);
        }

        /// <summary>
        /// Return <see cref="IMatchingCharacter"/> for supplied character id.
        /// </summary>
        /// <remarks>
        /// Will return a new <see cref="IMatchingCharacter"/> if one does not exist.
        public IMatchingCharacter GetMatchingCharacter(ulong characterId)
        {
            if (!characters.TryGetValue(characterId, out IMatchingCharacter characterInfo))
            {
                characterInfo = matchingCharacterFactory.Resolve();
                characterInfo.Initialise(characterId);
                characters.Add(characterId, characterInfo);
            }

            return characterInfo;
        }

        /// <summary>
        /// Return <see cref="IMatchingRoleCheck"/> for supplied character id.
        /// </summary>
        public IMatchingRoleCheck GetMatchingRoleCheck(ulong characterId)
        {
            return characterMatchingRoleChecks.TryGetValue(characterId, out IMatchingRoleCheck matchingRoleCheck) ? matchingRoleCheck : null;
        }

        /// <summary>
        /// Attempt to join a matching queue.
        /// </summary>
        public void JoinQueue(IPlayer player, Role roles, Static.Matching.MatchType matchType, List<uint> maps, uint matchingGameTypeId, MatchingQueueFlags matchingQueueFlags)
        {
            log.LogTrace($"Queue join request, Character: {player.CharacterId}, Roles: {roles}, MatchType: {matchType}, Maps: {string.Join(", ", maps)}, Type {matchingGameTypeId}, Flags: {matchingQueueFlags}.");

            List<IMatchingMap> matchingMaps = GetMatchingMaps(maps, matchingGameTypeId);
            JoinQueue(player, roles, matchType, matchingMaps, matchingQueueFlags);
        }

        /// <summary>
        /// Attempt to join a matching queue with a party.
        /// </summary>
        public void JoinPartyQueue(IPlayer player, Role roles, Static.Matching.MatchType matchType, List<uint> maps, uint matchingGameTypeId, MatchingQueueFlags matchingQueueFlags)
        {
            log.LogTrace($"Party queue join request, Character: {player.CharacterId}, Roles: {roles}, MatchType: {matchType}, Maps: {string.Join(", ", maps)}, Type {matchingGameTypeId}, Flags: {matchingQueueFlags}.");

            List<IMatchingMap> matchingMaps = GetMatchingMaps(maps, matchingGameTypeId);
            JoinPartyQueue(player, roles, matchType, matchingMaps, matchingQueueFlags);
        }

        private List<IMatchingMap> GetMatchingMaps(List<uint> maps, uint matchingGameTypeId)
        {
            // arenas also specify the MatchingGameType
            // this is because the MatchType is not enough to determine the type (1v1, 3v3, 5v5)
            if (matchingGameTypeId != 0)
                return matchingDataManager.GetMatchingMaps(matchingGameTypeId)
                    .ToList();

            return maps
                .Select(matchingDataManager.GetMatchingMap)
                .ToList();
        }

        private void JoinQueue(IPlayer player, Role roles, Static.Matching.MatchType matchType, List<IMatchingMap> matchingMaps, MatchingQueueFlags matchingQueueFlags)
        {
            IMatchingQueueProposal matchingQueueProposal = matchingQueueProposalFactory.Resolve();
            matchingQueueProposal.Initialise(player.Faction1, matchType, matchingMaps, matchingQueueFlags);
            matchingQueueProposal.AddMember(player.CharacterId, roles);
            incomingMatchingQueueProposals.Enqueue(matchingQueueProposal);
        }

        private void JoinPartyQueue(IPlayer player, Role roles, Static.Matching.MatchType matchType, List<IMatchingMap> matchingMaps, MatchingQueueFlags matchingQueueFlags)
        {
            IMatchingQueueProposal matchingQueueProposal = matchingQueueProposalFactory.Resolve();
            matchingQueueProposal.Initialise(player.Faction1, matchType, matchingMaps, matchingQueueFlags);

            // blame Max for this...
            // since we don't have party support yet, just do a sneaky grid search for players
            List<ulong> characterIds = player.Map
                .Search(player.Position, 10f, new SearchCheckRange<IPlayer>(player.Position, 10f))
                .Select(p => p.CharacterId)
                .ToList();

            IMatchingRoleCheck matchingRoleCheck = matchingRoleCheckFactory.Resolve();
            matchingRoleCheck.Initialise(matchingQueueProposal, characterIds);

            matchingRoleChecks.Add(matchingRoleCheck);
            foreach (ulong characterId in characterIds)
                characterMatchingRoleChecks.Add(characterId, matchingRoleCheck);

            log.LogTrace($"Role check {matchingRoleCheck.Guid} added to store.");
        }

        private void JoinQueue(IMatchingQueueProposal matchingQueueProposal)
        {
            IMatchingQueueManager matchingQueueManager = GetMatchingQueueManager(matchingQueueProposal.Faction, matchingQueueProposal.MatchType);

            MatchingQueueResult? matchingResult = matchingQueueManager.CanQueue(matchingQueueProposal);
            if (matchingResult != null)
            {
                matchingQueueProposal.Broadcast(new ServerMatchingQueueResult
                {
                    Result = matchingResult.Value
                });

                log.LogTrace($"Matching queue proposal {matchingQueueProposal.Guid} failed to validate, reason: {matchingResult}.");
                return;
            }

            matchingQueueManager.JoinQueue(matchingQueueProposal);
        }

        private IMatchingQueueManager GetMatchingQueueManager(Faction faction, Static.Matching.MatchType matchType)
        {
            if (matchingDataManager.IsPvPMatchType(matchType))
                return pvpMatchingQueueManager;

            return pveMatchingQueueManagers[faction];
        }

        /// <summary>
        /// Attempt to join a random queue.
        /// </summary>
        public void JoinRandomQueue(IPlayer player, Role roles, Static.Matching.MatchType matchType)
        {
            log.LogTrace($"Random queue join request, Character: {player.CharacterId}, Roles: {roles}, MatchType:, {matchType}.");

            List<IMatchingMap> maps = matchingDataManager.GetMatchingMaps(matchType).ToList();
            JoinQueue(player, roles, matchType, maps, MatchingQueueFlags.None);
        }

        /// <summary>
        /// Attempt to join a random party queue.
        /// </summary>
        public void JoinRandomPartyQueue(IPlayer player, Role roles, Static.Matching.MatchType matchType)
        {
            log.LogTrace($"Random party queue join request, Character: {player.CharacterId}, Roles: {roles}, MatchType:, {matchType}.");

            List<IMatchingMap> maps = matchingDataManager.GetMatchingMaps(matchType).ToList();
            JoinPartyQueue(player, roles, matchType, maps, MatchingQueueFlags.None);
        }

        /// <summary>
        /// Remove <see cref="IPlayer"/> from specified <see cref="Static.Matching.MatchType"/> queue.
        /// </summary>
        public void LeaveQueue(IPlayer player, Static.Matching.MatchType matchType)
        {
            log.LogTrace($"Leave queue request, Character: {player.CharacterId}, MatchType: {matchType}.");

            IMatchingCharacter character = GetMatchingCharacter(player.CharacterId);
            IMatchingCharacterQueue matchingCharacterQueue = character.GetMatchingCharacterQueue(matchType);
            matchingCharacterQueue.MatchingQueueGroup.RemoveMatchingQueueProposal(matchingCharacterQueue.MatchingQueueProposal);
        }

        /// <summary>
        /// Remove <see cref="IPlayer"/> from all queues.
        /// </summary>
        public void LeaveQueue(IPlayer player)
        {
            log.LogTrace($"Leave queue request, Character: {player.CharacterId}.");

            IMatchingCharacter character = GetMatchingCharacter(player.CharacterId);
            foreach (IMatchingCharacterQueue matchingCharacterQueue in character.GetMatchingCharacterQueues())
                matchingCharacterQueue.MatchingQueueGroup.RemoveMatchingQueueProposal(matchingCharacterQueue.MatchingQueueProposal);
        }

        /// <summary>
        /// Invoked when <see cref="IPlayer"/> goes offline.
        /// </summary>
        public void OnLogout(IPlayer player)
        {
            GetMatchingRoleCheck(player.CharacterId)?.Respond(player.CharacterId, Role.None);
        }
    }
}
