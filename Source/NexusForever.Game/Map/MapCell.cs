using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Abstract.Map.Search;
using NexusForever.IO.Map;
using NLog;

namespace NexusForever.Game.Map
{
    public class MapCell : IMapCell
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Coordinates of cell within the grid.
        /// </summary>
        public (uint X, uint Z) Coord { get; }

        private readonly HashSet<IGridEntity> entities = new();

        /// <summary>
        /// Return <see cref="IMapCell"/> at supplied <see cref="Vector3"/>.
        /// </summary>
        public static (uint cellX, uint cellZ) GetCellCoord(Vector3 vector)
        {
            int x = (int)Math.Floor(MapDefines.GridCellCount * (MapDefines.WorldGridOrigin + vector.X / MapDefines.GridSize));
            if (x < 0 || x > MapDefines.WorldGridCount * MapDefines.GridCellCount)
                throw new ArgumentOutOfRangeException($"Position X: {vector.X} is invalid!");

            int z = (int)Math.Floor(MapDefines.GridCellCount * (MapDefines.WorldGridOrigin + vector.Z / MapDefines.GridSize));
            if (z < 0 || z > MapDefines.WorldGridCount * MapDefines.GridCellCount)
                throw new ArgumentOutOfRangeException($"Position Z: {vector.X} is invalid!");

            return ((uint)x & MapDefines.GridCellCount - 1, (uint)z & MapDefines.GridCellCount - 1);
        }

        /// <summary>
        /// Initialise new <see cref="IMapCell"/> at the supplied position.
        /// </summary>
        public MapCell(uint cellX, uint cellZ)
        {
            Coord = (cellX, cellZ);
        }

        public void Update(double lastTick)
        {
            foreach (IGridEntity entity in entities)
                entity.Update(lastTick);
        }

        /// <summary>
        /// Unload <see cref="IGridEntity"/>'s from the <see cref="IMapCell"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="IGridEntity"/>'s are delay unloaded.
        /// </remarks>
        public void Unload()
        {
            foreach (IGridEntity entity in entities)
                entity.RemoveFromMap();
        }

        /// <summary>
        /// Add <see cref="IGridEntity"/> to the <see cref="IMapCell"/>.
        /// </summary>
        public void AddEntity(IGridEntity entity)
        {
            entities.Add(entity);
            log.Trace($"Added entity {entity.Guid} to cell at X:{Coord.X}, Z:{Coord.Z}.");
        }

        /// <summary>
        /// Remove <see cref="IGridEntity"/> from the <see cref="IMapCell"/>.
        /// </summary>
        public void RemoveEntity(IGridEntity entity)
        {
            entities.Remove(entity);
            log.Trace($"Removed entity {entity.Guid} from cell at X:{Coord.X}, Z:{Coord.Z}.");
        }

        /// <summary>
        /// Return all <see cref="IGridEntity"/>'s in the <see cref="IMapCell"/> that satisfy <see cref="ISearchCheck"/>.
        /// </summary>
        public void Search(ISearchCheck check, List<IGridEntity> intersectedEntities)
        {
            intersectedEntities.AddRange(entities.Where(check.CheckEntity));
        }
    }
}
