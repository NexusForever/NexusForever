using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map.Search;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Map
{
    public interface IMapCell : IUpdate
    {
        /// <summary>
        /// Coordinates of cell within the grid.
        /// </summary>
        (uint X, uint Z) Coord { get; }

        /// <summary>
        /// Unload <see cref="IGridEntity"/>'s from the <see cref="IMapCell"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="IGridEntity"/>'s are delay unloaded.
        /// </remarks>
        void Unload();

        /// <summary>
        /// Add <see cref="IGridEntity"/> to the <see cref="IMapCell"/>.
        /// </summary>
        void AddEntity(IGridEntity entity);

        /// <summary>
        /// Remove <see cref="IGridEntity"/> from the <see cref="IMapCell"/>.
        /// </summary>
        void RemoveEntity(IGridEntity entity);

        /// <summary>
        /// Return all <see cref="IGridEntity"/>'s in the <see cref="IMapCell"/> that satisfy <see cref="ISearchCheck"/>.
        /// </summary>
        void Search(ISearchCheck check, List<IGridEntity> intersectedEntities);
    }
}