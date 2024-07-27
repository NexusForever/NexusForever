using System.Numerics;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Abstract.Matching.Queue;
using NexusForever.Network.Message;

namespace NexusForever.Game.Abstract.Matching.Match
{
    public interface IMatchTeamMember
    {
        ulong CharacterId { get; }
        bool InMatch { get; }
        IMapPosition ReturnPosition { get; }
        Vector3 ReturnRotation { get; }

        /// <summary>
        /// Initialise new <see cref="IMatchTeamMember"/> with supplied <see cref="IMatchingQueueProposalMember"/>.
        /// </summary>
        void Initialise(IMatchingQueueProposalMember matchingQueueProposalMember);

        /// <summary>
        /// Invoked when member enters the match.
        /// </summary>
        void MatchEnter(IMatchingMap matchingMap);

        /// <summary>
        /// Invoked when member exits the match.
        /// </summary>
        void MatchExit();

        /// <summary>
        /// Teleport member to match.
        /// </summary>
        void TeleportToMatch(IMapEntrance mapEntrance);

        /// <summary>
        /// Teleport member to return location.
        /// </summary>
        /// <remarks>
        /// Return position is the position of the character before entering the match.
        /// </remarks>
        void TeleportToReturn();

        /// <summary>
        /// Send <see cref="IWritable"/> to member.
        /// </summary>
        void Send(IWritable message);
    }
}
