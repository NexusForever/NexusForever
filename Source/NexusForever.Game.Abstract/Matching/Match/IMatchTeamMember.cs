using System.Numerics;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Static.Matching;
using NexusForever.Network.Message;

namespace NexusForever.Game.Abstract.Matching.Match
{
    public interface IMatchTeamMember
    {
        Identity Identity { get; }
        bool InMatch { get; }
        Role Roles { get; }
        IMapPosition ReturnPosition { get; }
        Vector3 ReturnRotation { get; }

        /// <summary>
        /// Initialise new <see cref="IMatchTeamMember"/> with supplied character id.
        /// </summary>
        void Initialise(Identity identity, Role roles);

        /// <summary>
        /// Invoked when member enters the match.
        /// </summary>
        void MatchEnter(IMatchingMap matchingMap);

        /// <summary>
        /// Invoked when member exits the match.
        /// </summary>
        void MatchExit(bool teleport);

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
