using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Abstract.Map.Instance;
using NexusForever.Game.Static.Matching;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Matching.Match
{
    public interface IMatch : IUpdate
    {
        Guid Guid { get; }
        MatchStatus Status { get; }
        IMatchingMap MatchingMap { get; }

        /// <summary>
        /// Initialise the match with the supplied <see cref="IMatchProposal"/>
        /// </summary>
        void Initialise(IMatchProposal matchProposal);

        /// <summary>
        /// Invoked when <see cref="IPlayer"/> logs in.
        /// </summary>
        void OnLogin(IPlayer player);

        /// <summary>
        /// Set <see cref="IContentMapInstance"/> the match is running on.
        /// </summary>
        void SetMap(IContentMapInstance contentMapInstance);

        /// <summary>
        /// Cleanup <see cref="IMatch"/>, removing all members, detatch map and finalising the match.
        /// </summary>
        void MatchCleanup();

        /// <summary>
        /// Return <see cref="IMatchTeam"/> for supplied character.
        /// </summary>
        IMatchTeam GetTeam(ulong characterId);

        /// <summary>
        /// Return a collection containing <see cref="IMatchTeam"/> in the match.
        /// </summary>
        IEnumerable<IMatchTeam> GetTeams();

        /// <summary>
        /// Invoked when <see cref="IPlayer"/> enters the match.
        /// </summary>
        void MatchEnter(IPlayer player);

        /// <summary>
        /// Invoked when <see cref="IPlayer"/> exits the match.
        /// </summary>
        void MatchExit(IPlayer player, bool teleport);

        /// <summary>
        /// Remove character from match.
        /// </summary>
        void MatchLeave(ulong characterId);

        /// <summary>
        /// Finish the match.
        /// </summary>
        void MatchFinish();

        /// <summary>
        /// Teleport supplied character to the match.
        /// </summary>
        void MatchTeleport(ulong characterId);

        /// <summary>
        /// Get return <see cref="IMapPosition"/> for <see cref="IPlayer"/>.
        /// </summary>
        /// <remarks>
        /// Return position is the position of the player before entering the match.
        /// </remarks>
        IMapPosition GetReturnPosition(IPlayer player);
    }
}
