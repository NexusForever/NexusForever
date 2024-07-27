using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Matching;
using NexusForever.Game.Abstract.Matching.Queue;
using NexusForever.Game.Static.Matching;
using NexusForever.Game.Static.Reputation;
using NexusForever.Shared;

namespace NexusForever.Game.Matching.Queue
{
    public class MatchingQueueGroup : IMatchingQueueGroup
    {
        public Guid Guid { get; } = Guid.NewGuid();
        public Static.Matching.MatchType MatchType { get; private set; }

        public bool IsFinalised { get; private set; }

        /// <summary>
        /// Determines if the <see cref="IMatchingQueueGroup"/> is paused.
        /// </summary>
        /// <remarks>
        /// When paused, the <see cref="IMatchingQueueGroup"/> will not be matched.
        /// </remarks>
        public bool IsPaused
        {
            get => isPaused;
            set
            {
                isPaused = value;
                log.LogTrace($"Matching queue group {Guid}, Paused: {value}.");
            }
        }

        private bool isPaused;

        /// <summary>
        /// Determines if the <see cref="IMatchingQueueGroup"/> is from a looking for replacements request.
        /// </summary>
        public bool InProgress { get; private set; }

        /// <summary>
        /// Determines if the <see cref="IMatchingQueueGroup"/> is from a solo queue join request.
        /// </summary>
        /// <remarks>
        /// This is only possible for Expeditions and is a UI option for players joining the queue.
        /// </remarks>
        public bool IsSolo { get; private set; }

        private readonly Dictionary<Faction, IMatchingQueueGroupTeam> teams = [];
        private List<IMatchingMap> maps = [];

        #region Dependency Injection

        private readonly ILogger<MatchingQueueGroup> log;

        private readonly IMatchingDataManager matchingDataManager;
        private readonly IFactory<IMatchingQueueGroupTeam> matchingQueueGroupTeamFactory;

        public MatchingQueueGroup(
            ILogger<MatchingQueueGroup> log,
            IMatchingDataManager matchingDataManager,
            IFactory<IMatchingQueueGroupTeam> matchingQueueGroupTeamFactory)
        {
            this.log                           = log;

            this.matchingDataManager           = matchingDataManager;
            this.matchingQueueGroupTeamFactory = matchingQueueGroupTeamFactory;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="IMatchingQueueGroup"/> with supplied <see cref="Static.Matching.MatchType"/> and <see cref="Faction"/>.
        /// </summary>
        public void Initialise(Static.Matching.MatchType matchType, Faction faction)
        {
            MatchType = matchType;

            AddTeam(faction);
            if (matchingDataManager.IsPvPMatchType(matchType))
                AddTeam(faction == Faction.Dominion ? Faction.Exile : Faction.Dominion);

            log.LogTrace($"Initialised new matching queue group, Guid: {Guid}, MatchType: {matchType}, Teams: {teams.Count}");
        }

        private void AddTeam(Faction faction)
        {
            IMatchingQueueGroupTeam matchingQueueGroupTeam = matchingQueueGroupTeamFactory.Resolve();
            matchingQueueGroupTeam.Faction = faction;
            teams.Add(faction, matchingQueueGroupTeam);

            log.LogTrace($"Added new team to matching queue group {Guid}, Faction: {faction}");
        }

        /// <summary>
        /// Return <see cref="IMatchingQueueGroupTeam"/> for the supplied <see cref="Faction"/>.
        /// </summary>
        public IMatchingQueueGroupTeam GetTeam(Faction faction)
        {
            return teams.TryGetValue(faction, out IMatchingQueueGroupTeam team) ? team : null;
        }

        public IEnumerable<IMatchingQueueGroupTeam> GetTeams()
        {
            return teams.Values;
        }

        /// <summary>
        /// Return common <see cref="IMatchingMap"/>'s between all <see cref="IMatchingQueueGroupTeam"/>'s.
        /// </summary>
        /// <remarks>
        /// When a new <see cref="IMatchingQueueProposal"/> is added or removed, this cache is updated.
        /// </remarks>
        public List<IMatchingMap> GetMatchingMaps()
        {
            return maps;
        }

        /// <summary>
        /// Add <see cref="IMatchingQueueProposal"/> to <see cref="IMatchingQueueGroup"/>.
        /// </summary>
        public void AddMatchingQueueProposal(IMatchingQueueProposal matchingQueueProposal)
        {
            if (IsSolo)
                throw new InvalidOperationException();

            if (matchingQueueProposal.MatchingQueueFlags.HasFlag(MatchingQueueFlags.SoloMatch))
                IsSolo = true;

            IMatchingQueueGroupTeam matchingQueueGroupTeam = GetTeam(matchingQueueProposal.Faction);
            matchingQueueGroupTeam.AddMatchingQueueProposal(matchingQueueProposal);
            matchingQueueProposal.SetMatchingQueueGroup(this);

            log.LogTrace($"Added matching queue proposal {matchingQueueProposal.Guid} to matching queue group {Guid}.");

            // cache maps
            maps = teams.Values
                .SelectMany(t => t.GetMatchingMaps())
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// Remove <see cref="IMatchingQueueProposal"/> from <see cref="IMatchingQueueGroup"/>, optionally with a <see cref="MatchingQueueResult"/>.
        /// </summary>
        public void RemoveMatchingQueueProposal(IMatchingQueueProposal matchingQueueProposal, MatchingQueueResult? leaveReason = MatchingQueueResult.Left)
        {
            IMatchingQueueGroupTeam matchingQueueGroupTeam = GetTeam(matchingQueueProposal.Faction);
            matchingQueueGroupTeam.RemoveMatchingQueueProposal(matchingQueueProposal);
            matchingQueueProposal.RemoveMatchingQueueGroup(leaveReason);

            log.LogTrace($"Removed matching queue proposal {matchingQueueProposal.Guid} from matching queue group {Guid}.");

            // cache maps
            maps = teams.Values
                .SelectMany(t => t.GetMatchingMaps())
                .Distinct()
                .ToList();

            if (maps.Count == 0)
            {
                IsFinalised = true;
                log.LogTrace($"Matching queue group {Guid} is empty, finalised.");
            }
        }
    }
}
