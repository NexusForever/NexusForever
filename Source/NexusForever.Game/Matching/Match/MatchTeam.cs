using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Abstract.Matching;
using NexusForever.Game.Abstract.Matching.Match;
using NexusForever.Game.Abstract.Matching.Queue;
using NexusForever.Game.Static.Reputation;
using NexusForever.Network.Message;
using NexusForever.Shared;

namespace NexusForever.Game.Matching.Match
{
    public class MatchTeam : IMatchTeam
    {
        public Faction Faction { get; private set; }

        private readonly Dictionary<ulong, IMatchTeamMember> members = [];
        private IMapEntrance mapEntrance;

        #region Dependency Injection

        private readonly IFactory<IMatchTeamMember> matchTeamMemberFactory;

        public MatchTeam(
            IFactory<IMatchTeamMember> matchTeamMemberFactory)
        {
            this.matchTeamMemberFactory = matchTeamMemberFactory;
        }

        #endregion

        /// <summary>
        /// Initialise new <see cref="IMatchTeam"/> with supplied <see cref="IMatchingQueueGroupTeam"/>.
        /// </summary>
        public void Initialise(IMatchingQueueGroupTeam matchingQueueGroupTeam, IMapEntrance mapEntrance)
        {
            if (this.mapEntrance != null)
                throw new InvalidOperationException();

            Faction = matchingQueueGroupTeam.Faction;

            this.mapEntrance = mapEntrance;

            foreach (IMatchingQueueProposalMember matchingQueueProposalMember in matchingQueueGroupTeam.GetMembers())
            {
                IMatchTeamMember matchTeamMember = matchTeamMemberFactory.Resolve();
                matchTeamMember.Initialise(matchingQueueProposalMember);
                matchTeamMember.TeleportToMatch(mapEntrance);

                members.Add(matchingQueueProposalMember.CharacterId, matchTeamMember);
            }
        }

        public IEnumerable<IMatchTeamMember> GetMembers()
        {
            return members.Values;
        }

        private IMatchTeamMember GetMember(ulong characterId)
        {
            return members.TryGetValue(characterId, out IMatchTeamMember matchTeamMember) ? matchTeamMember : null;
        }

        /// <summary>
        /// Invoked when character enters the match.
        /// </summary>
        public void MatchEnter(ulong characterId, IMatchingMap matchingMap)
        {
            IMatchTeamMember matchTeamMember = GetMember(characterId);
            matchTeamMember?.MatchEnter(matchingMap);
        }

        /// <summary>
        /// Invoked when character exist the match.
        /// </summary>
        public void MatchExit(ulong characterId)
        {
            IMatchTeamMember matchTeamMember = GetMember(characterId);
            matchTeamMember?.MatchExit();
        }

        /// <summary>
        /// Get return <see cref="IMapPosition"/> for character.
        /// </summary>
        /// <remarks>
        /// Return position is the position of the character before entering the match.
        /// </remarks>
        public IMapPosition GetReturnPosition(ulong characterId)
        {
            IMatchTeamMember matchTeamMember = GetMember(characterId);
            return matchTeamMember?.ReturnPosition;
        }

        /// <summary>
        /// Broadcast <see cref="IWritable"/> to all members.
        /// </summary>
        public void Broadcast(IWritable message)
        {
            foreach (IMatchTeamMember matchTeamMember in members.Values)
                matchTeamMember.Send(message);
        }
    }
}
