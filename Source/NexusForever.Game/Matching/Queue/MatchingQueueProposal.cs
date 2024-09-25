using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Matching;
using NexusForever.Game.Abstract.Matching.Queue;
using NexusForever.Game.Static.Matching;
using NexusForever.Game.Static.Reputation;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Shared;

namespace NexusForever.Game.Matching.Queue
{
    public class MatchingQueueProposal : IMatchingQueueProposal
    {
        public Guid Guid { get; private set; }
        public Faction Faction { get; private set; }
        public Static.Matching.MatchType MatchType { get; private set; }
        public MatchingQueueFlags MatchingQueueFlags { get; private set; }
        public DateTime QueueTime { get; private set; }

        public bool InstantQueue { get; private set; }

        /// <summary>
        /// Determines if the <see cref="IMatchingQueueProposal"/> was formed from a party.
        /// </summary>
        public bool IsParty => members.Count > 1;

        private readonly Dictionary<ulong, IMatchingQueueProposalMember> members = [];
        private readonly Dictionary<uint, IMatchingMap> maps = [];

        private IMatchingQueueGroup matchingQueueGroup;

        #region Dependency Injection

        private readonly ILogger<MatchingQueueProposal> log;

        private readonly IMatchingManager matchingManager;
        private readonly IMatchingQueueTimeManager matchingQueueTime;
        private readonly IFactory<IMatchingQueueProposalMember> memberFactory;
        private readonly IMatchingDataManager matchingDataManager;

        public MatchingQueueProposal(
            ILogger<MatchingQueueProposal> log,
            IMatchingManager matchingManager,
            IMatchingQueueTimeManager matchingQueueTime,
            IFactory<IMatchingQueueProposalMember> memberFactory,
            IMatchingDataManager matchingDataManager)
        {
            this.log                 = log;

            this.matchingManager     = matchingManager;
            this.matchingQueueTime   = matchingQueueTime;
            this.memberFactory       = memberFactory;
            this.matchingDataManager = matchingDataManager;
        }

        #endregion

        /// <summary>
        /// Initialise a new <see cref="IMatchingQueueProposal"/>.
        /// </summary>
        public void Initialise(Faction faction, Static.Matching.MatchType matchType, IEnumerable<IMatchingMap> matchingMaps, MatchingQueueFlags matchingQueueFlags)
        {
            if (Guid != Guid.Empty)
                throw new InvalidOperationException();

            Guid               = Guid.NewGuid();
            Faction            = faction;
            MatchType          = matchType;
            MatchingQueueFlags = matchingQueueFlags;

            foreach (IMatchingMap map in matchingMaps)
                maps.Add(map.Id, map);

            QueueTime    = DateTime.UtcNow;
            InstantQueue = CanInstantQueue();

            log.LogTrace($"Initialised matching queue proposal {Guid}, Faction: {Faction}, MatchType: {MatchType}, Maps: {string.Join(", ", maps)}, Flags: {matchingQueueFlags}.");
        }

        private bool CanInstantQueue()
        {
            if (MatchingQueueFlags.HasFlag(MatchingQueueFlags.SoloMatch))
                return true;

            if (matchingDataManager.DebugInstantQueue)
                return true;

            return false;
        }

        /// <summary>
        /// Add a new member to the <see cref="IMatchingQueueProposal"/> with supplied character id and <see cref="Role"/>.
        /// </summary>
        public void AddMember(ulong characterId, Role roles)
        {
            IMatchingQueueProposalMember matchingQueueProposalMember = memberFactory.Resolve();
            matchingQueueProposalMember.Initialise(this, characterId, roles);
            members.Add(characterId, matchingQueueProposalMember);

            log.LogTrace($"Added member {characterId} to matching queue proposal {Guid}.");
        }

        /// <summary>
        /// Return <see cref="IMatchingQueueProposalMember"/> for the supplied character id.
        /// </summary>
        public IMatchingQueueProposalMember GetMember(ulong characterId)
        {
            return members.TryGetValue(characterId, out IMatchingQueueProposalMember member) ? member : null;
        }

        public IEnumerable<IMatchingQueueProposalMember> GetMembers()
        {
            return members.Values;
        }

        public IEnumerable<IMatchingMap> GetMatchingMaps()
        {
            return maps.Values;
        }

        /// <summary>
        /// Set <see cref="IMatchingQueueGroup"/> for the <see cref="IMatchingQueueProposal"/>.
        /// </summary>
        public void SetMatchingQueueGroup(IMatchingQueueGroup matchingQueueGroup)
        {
            if (this.matchingQueueGroup != null)
                throw new InvalidOperationException();

            this.matchingQueueGroup = matchingQueueGroup;

            Broadcast(BuildServerMatchingQueueJoin());

            foreach (IMatchingQueueProposalMember matchingQueueProposalMember in GetMembers())
            {
                IMatchingCharacter character = matchingManager.GetMatchingCharacter(matchingQueueProposalMember.CharacterId);
                character.AddMatchingQueueProposal(this, matchingQueueGroup);
            }

            log.LogTrace($"Set matching queue group {matchingQueueGroup.Guid} for matching queue proposal {Guid}.");
        }

        /// <summary>
        /// Remove <see cref="IMatchingQueueGroup"/> from <see cref="IMatchingQueueProposal"/>, optionally with a <see cref="MatchingQueueResult"/>.
        /// </summary>
        public void RemoveMatchingQueueGroup(MatchingQueueResult? leaveReason = MatchingQueueResult.Left)
        {
            if (matchingQueueGroup == null)
                throw new InvalidOperationException();

            foreach (IMatchingQueueProposalMember matchingQueueProposalMember in GetMembers())
            {
                IMatchingCharacter character = matchingManager.GetMatchingCharacter(matchingQueueProposalMember.CharacterId);
                character.RemoveMatchingQueueProposal(matchingQueueGroup.MatchType, leaveReason);
            }

            log.LogTrace($"Removed matching queue group {matchingQueueGroup.Guid} from matching queue proposal {Guid}.");
            matchingQueueGroup = null;
        }

        private IWritable BuildServerMatchingQueueJoin()
        {
            return new ServerMatchingQueueJoin()
            {
                MapData = new()
                {
                    MatchType  = MatchType,
                    Maps       = maps.Values.Select(m => m.Id).ToList(),
                    QueueFlags = MatchingQueueFlags
                },
                QueueData = new()
                {
                    MatchType         = MatchType,
                    IsParty           = IsParty,
                    QueueTime         = (uint)(DateTime.UtcNow - QueueTime).TotalMilliseconds,
                    EstimatedWaitTime = (uint)matchingQueueTime.GetAdverageWaitTime(MatchType).TotalMilliseconds
                },
            };
        }

        /// <summary>
        /// Broadcast <see cref="IWritable"/> to all members.
        /// </summary>
        public void Broadcast(IWritable message)
        {
            foreach (IMatchingQueueProposalMember member in members.Values)
                member.Send(message);
        }
    }
}
