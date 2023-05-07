using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Abstract.Map.Search;
using NexusForever.Game.Configuration.Model;
using NexusForever.Game.Static.Map;
using NexusForever.IO.Map;
using NexusForever.Shared.Configuration;
using NexusForever.Shared.Game;
using NLog;

namespace NexusForever.Game.Map
{
    public class MapGrid : IMapGrid
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Coordinates of grid within the map.
        /// </summary>
        public (uint X, uint Z) Coord { get; }

        /// <summary>
        /// Current unload status for map grid.
        /// </summary>
        /// <remarks>
        /// Status will only be set if the grid is in an unloading state.
        /// </remarks>
        public MapUnloadStatus? UnloadStatus { get; private set; }

        private readonly IMapCell[] cells = new MapCell[MapDefines.GridCellCount * MapDefines.GridCellCount];

        private uint visibilityCount;
        private uint entityCount;
        private readonly UpdateTimer unloadTimer;

        /// <summary>
        /// Return <see cref="IMapGrid"/> at supplied <see cref="Vector3"/>.
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
        /// Initialise new <see cref="IMapGrid"/> at the supplied position.
        /// </summary>
        public MapGrid(uint gridX, uint gridZ, bool allowUnload)
        {
            Coord = (gridX, gridZ);

            for (uint z = 0u; z < MapDefines.GridCellCount; z++)
                for (uint x = 0u; x < MapDefines.GridCellCount; x++)
                    cells[z * MapDefines.GridCellCount + x] = new MapCell(x, z);

            if (allowUnload)
                unloadTimer = new UpdateTimer(SharedConfiguration.Instance.Get<MapConfig>().GridUnloadTimer ?? 600d);
        }

        public void Update(double lastTick)
        {
            if (!UnloadStatus.HasValue)
                foreach (IMapCell cell in cells)
                    cell.Update(lastTick);

            ProcessUnload(lastTick);
        }

        private void ProcessUnload(double lastTick)
        {
            switch (UnloadStatus)
            {
                case MapUnloadStatus.AwaitingUnloadEntities:
                {
                    foreach (IMapCell cell in cells)
                        cell.Unload();

                    UnloadStatus = MapUnloadStatus.UnloadingEntities;
                    break;
                }
                case MapUnloadStatus.UnloadingEntities:
                {
                    if (entityCount == 0)
                        UnloadStatus = MapUnloadStatus.Complete;
                    break;
                }
                case null when unloadTimer != null:
                {
                    unloadTimer.Update(lastTick);
                    if (unloadTimer.HasElapsed)
                    {
                        unloadTimer.Reset(false);
                        Unload();
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Unload <see cref="IMapCell"/>'s from the <see cref="IMapGrid"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="IMapCell"/>'s are delay unloaded.
        /// </remarks>
        public void Unload()
        {
            if (visibilityCount > 0u)
                throw new InvalidOperationException("Failed to start grid unload, players in visibility range!");

            UnloadStatus = entityCount == 0 ? MapUnloadStatus.Complete : MapUnloadStatus.AwaitingUnloadEntities;
        }

        /// <summary>
        /// Notify <see cref="IMapGrid"/> of the addition of new <see cref="IPlayer"/> that is in vision range.
        /// </summary>
        public void AddVisiblePlayer()
        {
            if (UnloadStatus.HasValue)
                return;

            checked
            {
                visibilityCount++;
            }

            // cancel grid unload timer when a new player comes into range
            if (unloadTimer != null && unloadTimer.IsTicking)
                unloadTimer.Reset(false);
        }

        /// <summary>
        /// Notify <see cref="IMapGrid"/> of the removal of an existing <see cref="IPlayer"/> that is no longer in vision range.
        /// </summary>
        public void RemoveVisiblePlayer()
        {
            checked
            {
                visibilityCount--;
            }

            // start map grid timer when the last player leaves range
            if (unloadTimer != null && visibilityCount <= 0u)
                unloadTimer.Reset();
        }

        /// <summary>
        /// Add <see cref="IGridEntity"/> to the <see cref="IMapGrid"/> at <see cref="Vector3"/>.
        /// </summary>
        public void AddEntity(IGridEntity entity, Vector3 vector)
        {
            if (UnloadStatus.HasValue)
                return;

            GetCell(vector).AddEntity(entity);

            checked
            {
                entityCount++;
            }

            log.Trace($"Added entity {entity.Guid} to grid at X:{Coord.X}, Z:{Coord.Z}.");
        }

        /// <summary>
        /// Remove <see cref="IGridEntity"/> from the <see cref="IMapGrid"/>.
        /// </summary>
        public void RemoveEntity(IGridEntity entity)
        {
            GetCell(entity.Position).RemoveEntity(entity);

            checked
            {
                entityCount--;
            }

            log.Trace($"Removed entity {entity.Guid} to grid at X:{Coord.X}, Z:{Coord.Z}.");
        }

        /// <summary>
        /// Relocate <see cref="IGridEntity"/> in the <see cref="IMapGrid"/> to <see cref="Vector3"/>.
        /// </summary>
        public void RelocateEntity(IGridEntity entity, Vector3 vector)
        {
            if (UnloadStatus.HasValue)
                return;

            IMapCell oldCell = GetCell(entity.Position);
            IMapCell newCell = GetCell(vector);

            // new position is in the same cell, no need to transfer entity to another cell
            if (newCell.Coord.X == oldCell.Coord.X
                && newCell.Coord.Z == oldCell.Coord.Z)
                return;

            oldCell.RemoveEntity(entity);
            newCell.AddEntity(entity);
        }

        /// <summary>
        /// Return all <see cref="IGridEntity"/>'s in grid that satisfy <see cref="ISearchCheck"/>.
        /// </summary>
        public void Search(Vector3 position, ISearchCheck check, List<IGridEntity> intersectedEntities)
        {
            GetCell(position).Search(check, intersectedEntities);
        }

        private IMapCell GetCell(Vector3 vector)
        {
            (uint cellX, uint cellZ) = MapCell.GetCellCoord(vector);
            return cells[cellZ * MapDefines.GridCellCount + cellX];
        }
    }
}
