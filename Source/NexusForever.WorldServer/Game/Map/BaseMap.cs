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
        public uint InstanceId { get; private set; }

        private readonly MapGrid[] grids = new MapGrid[MapDefines.WorldGridCount * MapDefines.WorldGridCount];
        private readonly HashSet<(uint GridX, uint GridZ)> activeGrids = new HashSet<(uint GridX, uint GridZ)>();

        private readonly ConcurrentQueue<IGridAction> pendingActions = new ConcurrentQueue<IGridAction>();

        private readonly QueuedCounter entityCounter = new QueuedCounter();
        private readonly Dictionary<uint /*guid*/, GridEntity> entities = new Dictionary<uint /*guid*/, GridEntity>();
        private EntityCache entityCache;

        public virtual void Initialise(MapInfo info, Player player)
        {
            Entry       = info.Entry;
            File        = BaseMapManager.Instance.GetBaseMap(Entry.AssetPath);
            InstanceId  = info.InstanceId;
            entityCache = EntityCacheManager.Instance.GetEntityCache((ushort)Entry.Id);
        }
        
        public virtual void Update(double lastTick)
        {
            uint actionThreshold = ConfigurationManager<WorldServerConfiguration>.Instance.Config.Map.GridActionThreshold ?? 100u;
            foreach (IGridAction action in pendingActions.Dequeue(actionThreshold))
            {
                switch (action)
                {
                    case GridActionAdd actionAdd:
                    {
                        if (!AddEntity(actionAdd.Entity, actionAdd.Vector))
                        {
                            // retry threshold to prevent and issues with stuck actions
                            actionAdd.RequeueCount++;
                            if (actionAdd.RequeueCount > 5u)
                                log.Error($"Failed to add entity to map {Entry.Id} at position X: {actionAdd.Vector.X}, Y: {actionAdd.Vector.Y}, Z: {actionAdd.Vector.Z}!");
                            else
                                pendingActions.Enqueue(action);
                        }

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

            var gridsToRemove = new HashSet<(uint GridX, uint GridZ)>();
            foreach ((uint gridX, uint gridZ) in activeGrids)
            {
                MapGrid grid = GetGrid(gridX, gridZ);

                grid.Update(lastTick);
                // make sure the grid has fully unloaded before removing from active grids
                if (grid.PendingUnload && DeactivateGrid(grid))
                    gridsToRemove.Add((gridX, gridZ));
            }

            foreach ((uint GridX, uint GridZ) coord in gridsToRemove)
                activeGrids.Remove(coord);
        }

        public virtual void OnAddToMap(Player player)
        {
        }

        /// <summary>
        /// Enqueue <see cref="GridEntity"/> to be added to <see cref="BaseMap"/> at <see cref="Vector3"/>.
        /// </summary>
        public void EnqueueAdd(GridEntity entity, Vector3 position)
        {
            entity.OnEnqueueAddToMap();
            pendingActions.Enqueue(new GridActionAdd(entity, position));
        }

        /// <summary>
        /// Enqueue <see cref="GridEntity"/> to be removed from <see cref="BaseMap"/>.
        /// </summary>
        public void EnqueueRemove(GridEntity entity)
        {
            entity.OnEnqueueRemoveFromMap();
            pendingActions.Enqueue(new GridActionRemove(entity));
        }

        /// <summary>
        /// Remove <see cref="GridEntity"/> from the <see cref="BaseMap"/>.
        /// </summary>
        /// <remarks>
        /// This will remove the entity right away, this should only be used in a few cases such as <see cref="MapGrid"/> unloading.
        /// </remarks>
        public void RemoveDirect(GridEntity entity)
        {
            RemoveEntity(entity);
        }

        /// <summary>
        /// Enqueue <see cref="GridEntity"/> to be relocated in <see cref="BaseMap"/> to <see cref="Vector3"/>.
        /// </summary>
        public void EnqueueRelocate(GridEntity entity, Vector3 position)
        {
            pendingActions.Enqueue(new GridActionRelocate(entity, position));
        }

        /// <summary>
        /// Return all <see cref="GridEntity"/>'s in map that satisfy <see cref="ISearchCheck"/>.
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
            var grid = new MapGrid(gridX, gridZ);
            grids[gridZ * MapDefines.WorldGridCount + gridX] = grid;
            activeGrids.Add(grid.Coord);

            log.Trace($"Activated grid at X:{gridX}, Z:{gridZ}.");

            foreach (EntityModel model in entityCache.GetEntities(gridX, gridZ))
            {
                // non issue once all entities types are handled
                WorldEntity entity = EntityManager.Instance.NewEntity((EntityType)model.Type) ?? EntityManager.Instance.NewEntity(EntityType.Simple);
                entity.Initialise(model);

                var vector = new Vector3(model.X, model.Y, model.Z);
                EnqueueAdd(entity, vector);
            }
        }

        /// <summary>
        /// Deactivate single <see cref="MapGrid"/>.
        /// </summary>
        private bool DeactivateGrid(MapGrid grid)
        {
            if (!grid.Unload())
                return false;

            grids[grid.Coord.Z * MapDefines.WorldGridCount + grid.Coord.X] = null;

            log.Trace($"Deactivated grid at X:{grid.Coord.X}, Z:{grid.Coord.Z}.");
            return true;
        }

        private bool AddEntity(GridEntity entity, Vector3 vector)
        {
            Debug.Assert(entity.Map == null);

            ActivateGrid(entity, vector);
            MapGrid grid = GetGrid(vector);

            // if the grid is unloading we can't add the new entity to it
            // we will need to wait for the grid to fully unload before adding
            // push the action to the back of the queue and try again in the future
            if (grid.PendingUnload)
                return false;

            grid.AddEntity(entity, vector);

            uint guid = entityCounter.Dequeue();
            entities.Add(guid, entity);
            entity.OnAddToMap(this, guid, vector);

            log.Trace($"Added entity {entity.Guid} to map {Entry.Id}.");
            return true;
        }

        private void RemoveEntity(GridEntity entity)
        {
            Debug.Assert(entity.Map != null);

            GetGrid(entity.Position).RemoveEntity(entity);

            log.Trace($"Removed entity {entity.Guid} from map {Entry.Id}.");

            entityCounter.Enqueue(entity.Guid);
            entities.Remove(entity.Guid);
            entity.OnRemoveFromMap();
        }

        private void RelocateEntity(GridEntity entity, Vector3 vector)
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
