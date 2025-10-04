using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Static.Matching;
using NexusForever.Game.Static.Reputation;
using NexusForever.Network.Message;

namespace NexusForever.Game.Abstract.Matching.Match
{
    public interface IMatchTeam
    {
        Static.Matching.MatchTeam Team { get; }

        /// <summary>
        /// The temporary secondary <see cref="Static.Reputation.Faction"/> for members during the match.
        /// </summary>
        /// <remarks>
        /// This is a special faction used only during matches, see <see cref="Faction.MatchingTeam1"/> and <see cref="Faction.MatchingTeam2"/>.
        /// </remarks>
        Faction Faction { get; }

        /// <summary>
        /// Initialise new <see cref="IMatchTeam"/> with supplied <see cref="Static.Matching.MatchTeam"/>.
        /// </summary>
        void Initialise(IMatch match, Static.Matching.MatchTeam team);

        /// <summary>
        /// Return <see cref="IMatchTeamMember"/> for supplied characterId.
        /// </summary>
        IMatchTeamMember GetMember(Identity identity);

        /// <summary>
        /// Return collection of all <see cref="IMatchTeamMember"/>'s in <see cref="IMatchTeam"/>.
        /// </summary>
        IEnumerable<IMatchTeamMember> GetMembers();

        /// <summary>
        /// Invoked when <see cref="IPlayer"/> logs in.
        /// </summary>
        void OnLogin(IPlayer player);

        /// <summary>
        /// Add character to team.
        /// </summary>
        void MatchJoin(Identity identity, Role roles);

        /// <summary>
        /// Invoked when character enters the match.
        /// </summary>
        void MatchEnter(Identity identity, IMatchingMap matchingMap);

        /// <summary>
        /// Invoked when character exist the match.
        /// </summary>
        void MatchExit(Identity identity, bool teleport);

        /// <summary>
        /// Invoked when character leaves the match.
        /// </summary>
        void MatchLeave(Identity identity);

        /// <summary>
        /// Teleport character to match.
        /// </summary>
        void MatchTeleport(Identity identity);

        /// <summary>
        /// Get return <see cref="IMapPosition"/> for character.
        /// </summary>
        /// <remarks>
        /// Return position is the position of the character before entering the match.
        /// </remarks>
        IMapPosition GetReturnPosition(Identity identity);

        /// <summary>
        /// Broadcast <see cref="IWritable"/> to all members.
        /// </summary>
        void Broadcast(IWritable message);
    }
}
