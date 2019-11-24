using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NexusForever.Shared;
using NexusForever.Shared.Game.Map;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Map.Search;
using NLog;

namespace NexusForever.WorldServer.Game.Map
{
    public class MapCell : IUpdate
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public (uint X, uint Z) Coord { get; }

        private readonly HashSet<GridEntity> entities = new HashSet<GridEntity>();

        /// <summary>
        /// Return <see cref="MapCell"/> at supplied <see cref="Vector3"/>.
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
        /// Initialise new <see cref="MapCell"/> at the supplied position.
        /// </summary>
        public MapCell(uint cellX, uint cellZ)
        {
            Coord = (cellX, cellZ);
        }

        public void Update(double lastTick)
        {
            foreach (GridEntity entity in entities)
                entity.Update(lastTick);
        }

        /// <summary>
        /// Unload <see cref="GridEntity"/>'s from the <see cref="MapCell"/>.
        /// </summary>
        public void Unload(ref uint threshHold)
        {
            foreach (GridEntity entity in entities.ToList())
            {
                entity.RemoveFromMapDirect();

                threshHold--;
                if (threshHold == 0u)
                    return;
            }
        }

        /// <summary>
        /// Add <see cref="GridEntity"/> to the <see cref="MapCell"/>.
        /// </summary>
        public void AddEntity(GridEntity entity)
        {
            entities.Add(entity);
            log.Trace($"Added entity {entity.Guid} to cell at X:{Coord.X}, Z:{Coord.Z}.");
        }

        /// <summary>
        /// Remove <see cref="GridEntity"/> from the <see cref="MapCell"/>.
        /// </summary>
        public void RemoveEntity(GridEntity entity)
        {
            entities.Remove(entity);
            log.Trace($"Removed entity {entity.Guid} from cell at X:{Coord.X}, Z:{Coord.Z}.");
        }

        /// <summary>
        /// Return all <see cref="GridEntity"/>'s in the <see cref="MapCell"/> that satisfy <see cref="ISearchCheck"/>.
        /// </summary>
        public void Search(ISearchCheck check, List<GridEntity> intersectedEntities)
        {
            intersectedEntities.AddRange(entities.Where(check.CheckEntity));
        }
    }
}
