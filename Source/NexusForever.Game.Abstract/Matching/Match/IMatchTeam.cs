using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Abstract.Matching.Queue;
using NexusForever.Game.Static.Reputation;
using NexusForever.Network.Message;

namespace NexusForever.Game.Abstract.Matching.Match
{
    public interface IMatchTeam
    {
        Faction Faction { get; }

        /// <summary>
        /// Initialise new <see cref="IMatchTeam"/> with supplied <see cref="IMatchingQueueGroupTeam"/>.
        /// </summary>
        void Initialise(IMatchingQueueGroupTeam matchingQueueGroupTeam, IMapEntrance mapEntrance);

        IEnumerable<IMatchTeamMember> GetMembers();

        /// <summary>
        /// Invoked when character enters the match.
        /// </summary>
        void MatchEnter(ulong characterId, IMatchingMap matchingMap);

        /// <summary>
        /// Invoked when character exist the match.
        /// </summary>
        void MatchExit(ulong characterId);

        /// <summary>
        /// Get return <see cref="IMapPosition"/> for character.
        /// </summary>
        /// <remarks>
        /// Return position is the position of the character before entering the match.
        /// </remarks>
        IMapPosition GetReturnPosition(ulong characterId);

        /// <summary>
        /// Broadcast <see cref="IWritable"/> to all members.
        /// </summary>
        void Broadcast(IWritable message);
    }
}
