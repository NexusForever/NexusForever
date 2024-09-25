using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Matching;

namespace NexusForever.Game.Abstract.Matching
{
    public interface IMatchingDataManager
    {
        public bool DebugInstantQueue { get; set; }

        /// <summary>
        /// Initialise <see cref="IMatchingDataManager"/> with data from the game tables.
        /// </summary>
        void Initialise();

        /// <summary>
        /// Return <see cref="IMatchingMap"/> with supplied id.
        /// </summary>
        IMatchingMap GetMatchingMap(uint matchingMapId);

        /// <summary>
        /// Return <see cref="IMatchingMap"/> with supplied matching game type id.
        /// </summary>
        IEnumerable<IMatchingMap> GetMatchingMaps(uint matchingGameTypeId);

        /// <summary>
        /// Return <see cref="IMatchingMap"/> with supplied <see cref="Static.Matching.MatchType"/>.
        /// </summary>
        IEnumerable<IMatchingMap> GetMatchingMaps(Static.Matching.MatchType matchType);

        /// <summary>
        /// Return if <see cref="Static.Matching.MatchType"/> is a PvP match type.
        /// </summary>
        bool IsPvPMatchType(Static.Matching.MatchType matchType);

        /// <summary>
        /// Return if <see cref="Static.Matching.MatchType"/> enforces a specific team composition.
        /// </summary>
        bool IsCompositionEnforced(Static.Matching.MatchType matchType);

        /// <summary>
        /// Return if <see cref="Static.Matching.MatchType"/> enforces a single faction queue.
        /// </summary>
        bool IsSingleFactionEnforced(Static.Matching.MatchType matchType);

        /// <summary>
        /// Return default <see cref="Role"/>s for the supplied <see cref="Class"/>.
        /// </summary>
        Role GetDefaultRole(Class @class);

        /// <summary>
        /// Return <see cref="IMapEntrance"/> for the supplied world and team.
        /// </summary>
        /// <remarks>
        /// Teams can have different entrances to the same map, for example in PvP.
        /// </remarks>
        IMapEntrance GetMapEntrance(uint worldId, byte team);

        /// <summary>
        /// Return if a player can re-enter a match for the supplied <see cref="Static.Matching.MatchType"/>.
        /// </summary>
        /// <remarks>
        /// The client uses this to determine if the "Teleport to Instance" button should be enabled.
        /// </remarks>
        bool CanReEnterMatch(Static.Matching.MatchType matchType);
    }
}
