using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using NexusForever.Database.World.Model;
using NexusForever.Shared;
using NexusForever.Shared.Configuration;
using NexusForever.Shared.Game.Map;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.IO.Map;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Map.Search;
using NexusForever.WorldServer.Game.Map.Static;
using NexusForever.WorldServer.Game.Static;
using NLog;

namespace NexusForever.WorldServer.Game.Map
{
    public class BaseMap : IMap
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public const float DefaultVisionRange = 128f;

        /// <summary>
        /// Distance between a <see cref="Player"/> and a <see cref="GridEntity"/> before the entity can be seen.
        /// </summary>
        public virtual float VisionRange { get; protected set; } = DefaultVisionRange;

        public WorldEntry Entry { get; private set; }
        public MapFile File { get; private set; }

        private readonly MapGrid[] grids = new MapGrid[MapDefines.WorldGridCount * MapDefines.WorldGridCount];
        private readonly HashSet<(uint GridX, uint GridZ)> activeGrids = new();

        protected readonly ConcurrentQueue<IGridAction> pendingActions = new();

        private readonly QueuedCounter entityCounter = new();
        protected readonly Dictionary<uint /*guid*/, GridEntity> entities = new();
        private EntityCache entityCache;

        /// <summary>
        /// Initialise <see cref="BaseMap"/> with <see cref="WorldEntry"/>.
        /// </summary>
        public virtual void Initialise(WorldEntry entry)
        {
            Entry       = entry;
            File        = BaseMapManager.Instance.GetBaseMap(Entry.AssetPath);
            entityCache = EntityCacheManager.Instance.GetEntityCache((ushort)Entry.Id);
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public virtual void Update(double lastTick)
        {
            ProcessGridActions();
            UpdateGrids(lastTick);
        }

        private void ProcessGridActions()
        {
            var newActions = new List<IGridAction>();
            foreach (IGridAction action in pendingActions.Dequeue(
                ConfigurationManager<WorldServerConfiguration>.Instance.Config.Map.GridActionThreshold ?? 100u))
            {
                switch (action)
                {
                    case GridActionAdd actionAdd:
                    {
                        if (CanAddEntity(actionAdd.Entity, actionAdd.Vector))
                            AddEntity(actionAdd.Entity, actionAdd.Vector);
                        else
                        {
                            // retry threshold to prevent any issues with stuck actions
                            actionAdd.RequeueCount++;
                            if (actionAdd.RequeueCount > (ConfigurationManager<WorldServerConfiguration>.Instance.Config.Map.GridActionMaxRetry ?? 5u))
                            {
                                log.Error($"Failed to add entity to map {Entry.Id} at position X: {actionAdd.Vector.X}, Y: {actionAdd.Vector.Y}, Z: {actionAdd.Vector.Z}!");
                            }
                            else
                                newActions.Add(action);
                        }

                        break;
                    }
                    case GridActionPending actionPending:
                    {
                        if (actionPending.Entity.Map == null)
                        {
                            newActions.Add(new GridActionAdd
                            {
                                Entity = actionPending.Entity,
                                Vector = actionPending.Vector
                            });
                        }
                        else
                            newActions.Add(actionPending);

                        break;
                    }
                    case GridActionRelocate actionRelocate:
                        RelocateEntity(actionRelocate.Entity, actionRelocate.Vector);
                        break;
                    case GridActionRemove actionRemove:
                        RemoveEntity(actionRemove.Entity);
                        break;
                }
            }

            // new actions are added to the queue after processing so they are processed starting next update
            foreach (IGridAction action in newActions)
                pendingActions.Enqueue(action);
        }

        private void UpdateGrids(double lastTick)
        {
            var unloadedGrids = new List<MapGrid>();
            foreach ((uint gridX, uint gridZ) in activeGrids)
            {
                MapGrid grid = GetGrid(gridX, gridZ);
                grid.Update(lastTick);

                if (grid.UnloadStatus == MapUnloadStatus.Complete)
                    unloadedGrids.Add(grid);
            }

            foreach (MapGrid grid in unloadedGrids)
            {
                grids[grid.Coord.Z * MapDefines.WorldGridCount + grid.Coord.X] = null;
                activeGrids.Remove(grid.Coord);

                log.Trace($"Deactivated grid at X:{grid.Coord.X}, Z:{grid.Coord.Z}.");
            }
        }

        /// <summary>
        /// Enqueue <see cref="GridEntity"/> to be added to <see cref="BaseMap"/>.
        /// </summary>
        /// <remarks>
        /// Characters should not be added directly through this method.
        /// Use <see cref="Player.TeleportTo(MapPosition, TeleportReason)"/> instead.
        /// </remarks>
        public void EnqueueAdd(GridEntity entity, MapPosition position)
        {
            entity.OnEnqueueAddToMap();

            if (entity.Map != null)
            {
                // entity is on an existing map, will need to be removed before add
                entity.RemoveFromMap();

                pendingActions.Enqueue(new GridActionPending
                {
                    Entity = entity,
                    Vector = position.Position
                });
            }
            else
            {
                pendingActions.Enqueue(new GridActionAdd
                {
                    Entity = entity,
                    Vector = position.Position
                });
            }
        }

        /// <summary>
        /// Returns if <see cref="GridEntity"/> can be added to <see cref="BaseMap"/>.
        /// </summary>
        public virtual bool CanEnter(GridEntity entity, MapPosition position)
        {
            if (!IsValidPosition(position.Position))
                return false;

            return true;
        }

        /// <summary>
        /// Returns if <see cref="Player"/> can be added to <see cref="BaseMap"/>.
        /// </summary>
        public virtual GenericError? CanEnter(Player player, MapPosition position)
        {
            if (!IsValidPosition(position.Position))
                return GenericError.InstanceInvalidDestination;

            return null;
        }

        private bool IsValidPosition(Vector3 position)
        {
            // make sure player position falls within a valid grid
            int radius = (MapDefines.GridSize * MapDefines.WorldGridCount) / 2;
            if (position.X < -radius || position.X > radius
                || position.Z < -radius || position.Z > radius)
                return false;

            // TODO: check if map file has a grid at position?

            return true;
        }

        /// <summary>
        /// Enqueue <see cref="GridEntity"/> to be removed from <see cref="BaseMap"/>.
        /// </summary>
        public void EnqueueRemove(GridEntity entity)
        {
            entity.OnEnqueueRemoveFromMap();
            pendingActions.Enqueue(new GridActionRemove
            {
                Entity = entity
            });
        }

        /// <summary>
        /// Enqueue <see cref="GridEntity"/> to be relocated in <see cref="BaseMap"/> to <see cref="Vector3"/>.
        /// </summary>
        public void EnqueueRelocate(GridEntity entity, Vector3 position)
        {
            pendingActions.Enqueue(new GridActionRelocate
            {
                Entity = entity,
                Vector = position
            });
        }

        /// <summary>
        /// Return all <see cref="GridEntity"/>'s from <see cref="Vector3"/> in range that satisfy <see cref="ISearchCheck"/>.
        /// </summary>
        public void Search(Vector3 vector, float radius, ISearchCheck check, out List<GridEntity> intersectedEntities)
        {
            // negative radius is unlimited distance
            if (radius < 0)
            {
                intersectedEntities = entities.Values.ToList();
                return;
            }

            intersectedEntities = new List<GridEntity>();
            for (float z = vector.Z - radius; z < vector.Z + radius + MapDefines.GridCellSize; z += MapDefines.GridCellSize)
            {
                for (float x = vector.X - radius; x < vector.X + radius + MapDefines.GridCellSize; x += MapDefines.GridCellSize)
                {
                    var searchVector = new Vector3(x, 0f, z);
                    // don't activate new grids during search
                    GetGrid(searchVector)?.Search(searchVector, check, intersectedEntities);
                }
            }
        }

        /// <summary>
        /// Return all <see cref="MapGrid"/>'s from <see cref="Vector3"/> in range.
        /// </summary>
        public void GridSearch(Vector3 vector, float radius, out List<MapGrid> intersectedGrids)
        {
            // negative radius is unlimited distance
            if (radius < 0)
            {
                intersectedGrids = GetActiveGrids().ToList();
                return;
            }

            intersectedGrids = new List<MapGrid>();
            for (float z = vector.Z - radius; z < vector.Z + radius + MapDefines.GridSize; z += MapDefines.GridSize)
            {
                for (float x = vector.X - radius; x < vector.X + radius + MapDefines.GridSize; x += MapDefines.GridSize)
                {
                    MapGrid grid = GetGrid(new Vector3(x, 0f, z));
                    if (grid != null)
                        intersectedGrids.Add(grid);
                }
            }
        }

        /// <summary>
        /// Return <see cref="GridEntity"/> by guid.
        /// </summary>
        public T GetEntity<T>(uint guid) where T : GridEntity
        {
            if (!entities.TryGetValue(guid, out GridEntity entity))
                return null;
            return (T)entity;
        }

        /// <summary>
        /// Enqueue broadcast of <see cref="IWritable"/> to all <see cref="Player"/>'s on the map.
        /// </summary>
        public void EnqueueToAll(IWritable message)
        {
            foreach (GridEntity entity in entities.Values)
            {
                Player player = entity as Player;
                player?.Session.EnqueueMessageEncrypted(message);
            }
        }

        /// <summary>
        /// Notify <see cref="MapGrid"/> at coordinates of the addition of new <see cref="Player"/> that is in vision range.
        /// </summary>
        public void GridAddVisiblePlayer(uint gridX, uint gridZ)
        {
            MapGrid grid = GetGrid(gridX, gridZ);
            grid.AddVisiblePlayer();
        }

        /// <summary>
        /// Notify <see cref="MapGrid"/> at coordinates of the removal of an existing <see cref="Player"/> that is no longer in vision range.
        /// </summary>
        public void GridRemoveVisiblePlayer(uint gridX, uint gridZ)
        {
            MapGrid grid = GetGrid(gridX, gridZ);
            grid.RemoveVisiblePlayer();
        }

        private MapGrid GetGrid(uint gridX, uint gridZ)
        {
            return grids[gridZ * MapDefines.WorldGridCount + gridX];
        }

        private MapGrid GetGrid(Vector3 vector)
        {
            (uint gridX, uint gridZ) = MapGrid.GetGridCoord(vector);
            return GetGrid(gridX, gridZ);
        }

        protected IEnumerable<MapGrid> GetActiveGrids()
        {
            return activeGrids
                .Select(g => GetGrid(g.GridX, g.GridZ));
        }

        /// <summary>
        /// Activate one or more <see cref="MapGrid"/>'s at <see cref="Vector"/> depending on vision range of <see cref="GridEntity"/>.
        /// </summary>
        private void ActivateGrid(GridEntity entity, Vector3 vector)
        {
            float range = entity.ActivationRange;
            for (float z = vector.Z - range; z < vector.Z + range + MapDefines.GridSize; z += MapDefines.GridSize)
            {
                for (float x = vector.X - range; x < vector.X + range + MapDefines.GridSize; x += MapDefines.GridSize)
                {
                    (uint gridX, uint gridZ) = MapGrid.GetGridCoord(new Vector3(x, 0f, z));
                    MapGrid grid = GetGrid(gridX, gridZ);
                    if (grid == null)
                        ActivateGrid(gridX, gridZ);
                }
            }
        }

        /// <summary>
        /// Activate single <see cref="MapGrid"/> at position.
        /// </summary>
        private void ActivateGrid(uint gridX, uint gridZ)
        {
            // instance grids are not unloaded, the entire instance is unloaded instead during inactivity
            var grid = new MapGrid(gridX, gridZ, this is not MapInstance);
            grids[gridZ * MapDefines.WorldGridCount + gridX] = grid;
            activeGrids.Add(grid.Coord);

            log.Trace($"Activated grid at X:{gridX}, Z:{gridZ}.");

            SpawnGrid(gridX, gridZ);
        }

        protected virtual void SpawnGrid(uint gridX, uint gridZ)
        {
            foreach (EntityModel model in entityCache.GetEntities(gridX, gridZ))
            {
                // non issue once all entities types are handled
                WorldEntity entity = EntityManager.Instance.NewEntity((EntityType)model.Type) ?? EntityManager.Instance.NewEntity(EntityType.Simple);
                entity.Initialise(model);

                var position = new MapPosition
                {
                    Position = new Vector3(model.X, model.Y, model.Z)
                };

                if (CanEnter(entity, position))
                    EnqueueAdd(entity, position);
            }
        }

        private bool CanAddEntity(GridEntity entity, Vector3 vector)
        {
            ActivateGrid(entity, vector);

            // if the grid doesn't exist we can't add the new entity to it
            MapGrid grid = GetGrid(vector);
            if (grid == null)
                return false;

            // if the grid is unloading we can't add the new entity to it
            // we will need to wait for the grid to fully unload
            if (grid.UnloadStatus.HasValue)
                return false;

            return true;
        }

        protected virtual void AddEntity(GridEntity entity, Vector3 vector)
        {
            Debug.Assert(entity.Map == null);

            MapGrid grid = GetGrid(vector);
            grid.AddEntity(entity, vector);

            uint guid = entityCounter.Dequeue();
            entities.Add(guid, entity);

            entity.OnAddToMap(this, guid, vector);

            log.Trace($"Added entity {entity.Guid} to map {Entry.Id}.");
        }

        protected virtual void RemoveEntity(GridEntity entity)
        {
            Debug.Assert(entity.Map != null);

            GetGrid(entity.Position).RemoveEntity(entity);

            log.Trace($"Removed entity {entity.Guid} from map {Entry.Id}.");

            entityCounter.Enqueue(entity.Guid);
            entities.Remove(entity.Guid);

            entity.OnRemoveFromMap();
        }

        protected virtual void RelocateEntity(GridEntity entity, Vector3 vector)
        {
            Debug.Assert(entity.Map != null);

            ActivateGrid(entity, vector);
            MapGrid newGrid = GetGrid(vector);
            MapGrid oldGrid = GetGrid(entity.Position);

            if (newGrid.Coord.X != oldGrid.Coord.X
                || newGrid.Coord.Z != oldGrid.Coord.Z)
            {
                oldGrid.RemoveEntity(entity);
                newGrid.AddEntity(entity, vector);
            }
            else
                oldGrid.RelocateEntity(entity, vector);

            entity.OnRelocate(vector);
        }

        /// <summary>
        /// Return terrain height at supplied position.
        /// </summary>
        public float GetTerrainHeight(float x, float z)
        {
            // TODO: handle cases for water and props
            return File.GetTerrainHeight(new Vector3(x, 0, z));
        }
    }
}
