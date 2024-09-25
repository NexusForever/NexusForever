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
        public Guid Guid { get; private set; }
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

        private readonly Dictionary<Guid, IMatchingQueueGroupTeam> teams = [];
        private readonly Dictionary<Faction, IMatchingQueueGroupTeam> factionTeams = [];

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
        public void Initialise(IMatchingQueueProposal matchingQueueProposal)
        {
            if (Guid != Guid.Empty)
                throw new InvalidOperationException();

            Guid      = Guid.NewGuid();
            MatchType = matchingQueueProposal.MatchType;

            if (matchingDataManager.IsSingleFactionEnforced(MatchType))
            {
                IMatchingQueueGroupTeam matchingQueueGroupTeam = AddTeam(matchingQueueProposal.Faction);
                AddMatchingQueueProposal(matchingQueueProposal, matchingQueueGroupTeam);

                if (matchingDataManager.IsPvPMatchType(MatchType))
                    AddTeam(matchingQueueGroupTeam.Faction == Faction.Dominion ? Faction.Exile : Faction.Dominion);
            }
            else
            {
                IMatchingQueueGroupTeam matchingQueueGroupTeam = AddTeam(null);
                AddMatchingQueueProposal(matchingQueueProposal, matchingQueueGroupTeam);

                if (matchingDataManager.IsPvPMatchType(MatchType))
                    AddTeam(null);
            }

            IsSolo = matchingQueueProposal.MatchingQueueFlags.HasFlag(MatchingQueueFlags.SoloMatch);

            log.LogTrace($"Initialised new matching queue group, Guid: {Guid}, MatchType: {MatchType}, Teams: {teams.Count}");
        }

        private IMatchingQueueGroupTeam AddTeam(Faction? faction)
        {
            IMatchingQueueGroupTeam matchingQueueGroupTeam = matchingQueueGroupTeamFactory.Resolve();
            matchingQueueGroupTeam.Initialise(faction);

            if (faction != null)
                factionTeams.Add(faction.Value, matchingQueueGroupTeam);

            teams.Add(matchingQueueGroupTeam.Guid, matchingQueueGroupTeam);
            log.LogTrace($"Added new team to matching queue group {Guid}, Faction: {faction}");

            return matchingQueueGroupTeam;
        }

        private IMatchingQueueGroupTeam GetTeam(Guid guid)
        {
            return teams.TryGetValue(guid, out IMatchingQueueGroupTeam team) ? team : null;
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
        public void AddMatchingQueueProposal(IMatchingQueueProposal matchingQueueProposal, IMatchingQueueGroupTeam matchingQueueGroupTeam)
        {
            if (IsSolo)
                throw new InvalidOperationException();

            if (GetTeam(matchingQueueGroupTeam.Guid) == null)
                throw new InvalidOperationException();

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
            foreach (IMatchingQueueGroupTeam matchingQueueGroupTeam in GetTeams())
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
