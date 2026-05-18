using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map.Search;
using NexusForever.Game.Static.Map;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Map
{
    public interface IMapGrid : IUpdate
    {
        /// <summary>
        /// Coordinates of grid within the map.
        /// </summary>
        (uint X, uint Z) Coord { get; }

        /// <summary>
        /// Current unload status for map grid.
        /// </summary>
        /// <remarks>
        /// Status will only be set if the grid is in an unloading state.
        /// </remarks>
        MapUnloadStatus? UnloadStatus { get; }

        /// <summary>
        /// Unload <see cref="IMapCell"/>'s from the <see cref="IMapGrid"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="IMapCell"/>'s are delay unloaded.
        /// </remarks>
        void Unload();

        /// <summary>
        /// Notify <see cref="IMapGrid"/> of the addition of new <see cref="IPlayer"/> that is in vision range.
        /// </summary>
        void AddVisiblePlayer();

        /// <summary>
        /// Notify <see cref="IMapGrid"/> of the removal of an existing <see cref="IPlayer"/> that is no longer in vision range.
        /// </summary>
        void RemoveVisiblePlayer();

        /// <summary>
        /// Add <see cref="IGridEntity"/> to the <see cref="IMapGrid"/> at <see cref="Vector3"/>.
        /// </summary>
        void AddEntity(IGridEntity entity, Vector3 vector);

        /// <summary>
        /// Remove <see cref="IGridEntity"/> from the <see cref="IMapGrid"/>.
        /// </summary>
        void RemoveEntity(IGridEntity entity);

        /// <summary>
        /// Relocate <see cref="IGridEntity"/> in the <see cref="IMapGrid"/> to <see cref="Vector3"/>.
        /// </summary>
        void RelocateEntity(IGridEntity entity, Vector3 vector);

        /// <summary>
        /// Return all <see cref="IGridEntity"/>'s in grid that satisfy <see cref="ISearchCheck{T}"/>.
        /// </summary>
        IEnumerable<T> Search<T>(Vector3 position, ISearchCheck<T> check) where T : IGridEntity;
    }
}