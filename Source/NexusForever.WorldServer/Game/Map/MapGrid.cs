using System;
using System.Collections.Generic;
using System.Numerics;
using NexusForever.Shared;
using NexusForever.Shared.Configuration;
using NexusForever.Shared.Game.Map;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Map.Search;
using NLog;

namespace NexusForever.WorldServer.Game.Map
{
    public class MapGrid : IUpdate
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public (uint X, uint Z) Coord { get; }

        public bool PendingUnload { get; private set; }

        private readonly MapCell[] cells = new MapCell[MapDefines.GridCellCount * MapDefines.GridCellCount];

        private uint playerCount;
        private readonly UpdateTimer unloadTimer = new UpdateTimer(ConfigurationManager<WorldServerConfiguration>.Instance.Config.Map.GridUnloadTimer ?? 600d);
        private uint unloadCellX;
        private uint unloadCellZ;

        /// <summary>
        /// Return <see cref="MapGrid"/> at supplied <see cref="Vector3"/>.
        /// </summary>
        public static (uint gridX, uint gridZ) GetGridCoord(Vector3 vector)
        {
            int gridX = MapDefines.WorldGridOrigin + (int)Math.Floor(vector.X / MapDefines.GridSize);
            if (gridX < 0 || gridX > MapDefines.WorldGridCount)
                throw new ArgumentOutOfRangeException($"Position X: {vector.X} is invalid!");

            int gridZ = MapDefines.WorldGridOrigin + (int)Math.Floor(vector.Z / MapDefines.GridSize);
            if (gridZ < 0 || gridZ > MapDefines.WorldGridCount)
                throw new ArgumentOutOfRangeException($"Position Z: {vector.Z} is invalid!");

            return ((uint)gridX, (uint)gridZ);
        }

        /// <summary>
        /// Initialise new <see cref="MapGrid"/> at the supplied position.
        /// </summary>
        public MapGrid(uint gridX, uint gridZ)
        {
            Coord = (gridX, gridZ);

            for (uint z = 0u; z < MapDefines.GridCellCount; z++)
                for (uint x = 0u; x < MapDefines.GridCellCount; x++)
                    cells[z * MapDefines.GridCellCount + x] = new MapCell(x, z);
        }

        public void Update(double lastTick)
        {
            foreach (MapCell cell in cells)
                cell.Update(lastTick);

            if (ConfigurationManager<WorldServerConfiguration>.Instance.Config.Map.GridUnloadTimer.HasValue)
            {
                unloadTimer.Update(lastTick);
                if (unloadTimer.HasElapsed)
                    PendingUnload = true;
            }
        }

        /// <summary>
        /// Notify <see cref="MapGrid"/> of the addition of new <see cref="Player"/> that is in vision range.
        /// </summary>
        public void AddVisiblePlayer()
        {
            playerCount++;
            if (unloadTimer.IsTicking)
                unloadTimer.Reset(false);
        }

        /// <summary>
        /// Notify <see cref="MapGrid"/> of the removal of an existing <see cref="Player"/> that is no longer in vision range.
        /// </summary>
        public void RemoveVisiblePlayer()
        {
            playerCount--;
            if (playerCount == 0u)
                unloadTimer.Reset();
        }

        /// <summary>
        /// Unload <see cref="MapCell"/>'s from the <see cref="MapGrid"/>.
        /// </summary>
        public bool Unload()
        {
            uint threshold = ConfigurationManager<WorldServerConfiguration>.Instance.Config.Map.GridActionThreshold ?? 100u;
            for (uint z = unloadCellZ; z < MapDefines.GridCellCount; z++)
            {
                for (uint x = unloadCellX; x < MapDefines.GridCellCount; x++)
                {
                    cells[z * MapDefines.GridCellCount + x].Unload(ref threshold);
                    if (threshold == 0u)
                    {
                        unloadCellX = x;
                        unloadCellZ = z;
                        return false;
                    }
                }

                unloadCellX = 0u;
            }
            
            return true;
        }

        /// <summary>
        /// Add <see cref="GridEntity"/> to the <see cref="MapGrid"/> at <see cref="Vector3"/>.
        /// </summary>
        public void AddEntity(GridEntity entity, Vector3 vector)
        {
            GetCell(vector).AddEntity(entity);
            log.Trace($"Added entity {entity.Guid} to grid at X:{Coord.X}, Z:{Coord.Z}.");
        }

        /// <summary>
        /// Remove <see cref="GridEntity"/> from the <see cref="MapGrid"/>.
        /// </summary>
        public void RemoveEntity(GridEntity entity)
        {
            GetCell(entity.Position).RemoveEntity(entity);
            log.Trace($"Removed entity {entity.Guid} to grid at X:{Coord.X}, Z:{Coord.Z}.");
        }

        /// <summary>
        /// Relocate <see cref="GridEntity"/> in the <see cref="MapGrid"/> to <see cref="Vector3"/>.
        /// </summary>
        public void RelocateEntity(GridEntity entity, Vector3 vector)
        {
            MapCell oldCell = GetCell(entity.Position);
            MapCell newCell = GetCell(vector);

            // new position is in the same cell, no need to transfer entity to another cell
            if (newCell.Coord.X == oldCell.Coord.X
                && newCell.Coord.Z == oldCell.Coord.Z)
                return;

            oldCell.RemoveEntity(entity);
            newCell.AddEntity(entity);
        }

        /// <summary>
        /// Return all <see cref="GridEntity"/>'s in grid that satisfy <see cref="ISearchCheck"/>.
        /// </summary>
        public void Search(Vector3 position, ISearchCheck check, List<GridEntity> intersectedEntities)
        {
            GetCell(position).Search(check, intersectedEntities);
        }

        private MapCell GetCell(Vector3 vector)
        {
            (uint cellX, uint cellZ) = MapCell.GetCellCoord(vector);
            return cells[cellZ * MapDefines.GridCellCount + cellX];
        }
    }
}
