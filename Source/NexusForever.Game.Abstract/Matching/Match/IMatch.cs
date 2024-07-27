using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Static.Matching;

namespace NexusForever.Game.Abstract.Matching.Match
{
    public interface IMatch
    {
        Guid Guid { get; }
        MatchStatus Status { get; set; }
        IMatchingMap MatchingMap { get; }

        /// <summary>
        /// Initialise the match with the supplied <see cref="IMatchProposal"/>
        /// </summary>
        void Initialise(IMatchProposal matchProposal);

        IEnumerable<IMatchTeam> GetTeams();

        /// <summary>
        /// Invoked when <see cref="IPlayer"/> enters the match.
        /// </summary>
        void MatchEnter(IPlayer player);

        /// <summary>
        /// Invoked when <see cref="IPlayer"/> exits the match.
        /// </summary>
        void MatchExit(IPlayer player);

        /// <summary>
        /// Get return <see cref="IMapPosition"/> for <see cref="IPlayer"/>.
        /// </summary>
        /// <remarks>
        /// Return position is the position of the player before entering the match.
        /// </remarks>
        IMapPosition GetReturnPosition(IPlayer player);
    }
}
