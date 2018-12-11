using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Database.World;
using NexusForever.WorldServer.Game.Entity;
using NLog;

namespace NexusForever.WorldServer.Game.Map
{
    public class BaseMap : IMap
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public const int GridCount  = 128;
        public const int GridOrigin = GridCount / 2;

        /// <summary>
        /// Adjacent grid distance to also activate in addition to source grid.
        /// </summary>
        public const int GridBounds = 1;

        // arbitrary
        public virtual float VisionRange { get; protected set; } = 32.0f;

        public WorldEntry Entry { get; private set; }
        public uint InstanceId { get; private set; }

        private readonly MapGrid[] grids = new MapGrid[GridCount * GridCount];
        private float viewDistance;

        private readonly ConcurrentQueue<GridAction> pendingAdd = new ConcurrentQueue<GridAction>();
        private readonly ConcurrentQueue<GridEntity> pendingRemove = new ConcurrentQueue<GridEntity>();
        private readonly ConcurrentQueue<GridAction> pendingRelocate = new ConcurrentQueue<GridAction>();

        private readonly QueuedCounter entityCounter = new QueuedCounter();

        private readonly Dictionary<uint, GridEntity> entities = new Dictionary<uint, GridEntity>();
        
        public virtual void Initialise(MapInfo info, Player player)
        {
            Entry      = info.Entry;
            InstanceId = info.InstanceId;
            CacheEntitySpawns();
        }
        
        private void CacheEntitySpawns()
        {
            foreach (var entity in WorldDatabase.GetEntities((ushort)Entry.Id))
            {
                // TODO: cache spawns by grid, only spawn when grid is loaded
                // (int X, int Z) coord = GetGridCoord(vector);

                var test   = new NonPlayer(entity);
                var vector = new Vector3(entity.X, entity.Y, entity.Z);
                EnqueueAdd(test, vector);
            }
        }

        /// <summary>
        /// Enqueue <see cref="GridEntity"/> to be added to map. 
        /// </summary>
        public void EnqueueAdd(GridEntity entity, Vector3 position)
        {
            pendingAdd.Enqueue(new GridAction(entity, position));
        }

        /// <summary>
        /// Enqueue <see cref="GridEntity"/> to be removed from map. 
        /// </summary>
        public void EnqueueRemove(GridEntity entity)
        {
            pendingRemove.Enqueue(entity);
        }

        /// <summary>
        /// Enqueue <see cref="GridEntity"/> to be relocated in map. 
        /// </summary>
        public void EnqueueRelocate(GridEntity entity, Vector3 position)
        {
            pendingRelocate.Enqueue(new GridAction(entity, position));
        }

        /// <summary>
        /// Return <see cref="MapGrid"/> for supplied position.
        /// </summary>
        private MapGrid GetGrid(Vector3 vector)
        {
            (int X, int Z) coord = GetGridCoord(vector);
            ActivateGrid(coord.X, coord.Z);
            return grids[coord.Z * GridCount + coord.X];
        }

        private (int X, int Z) GetGridCoord(Vector3 vector)
        {
            int gridX = GridOrigin + (int)Math.Floor(vector.X / MapGrid.Size);
            if (gridX < 0 || gridX > GridCount)
                throw new ArgumentOutOfRangeException($"Position X: {vector.X} is invalid for grid!");

            int gridZ = GridOrigin + (int)Math.Floor(vector.Z / MapGrid.Size);
            if (gridZ < 0 || gridZ > GridCount)
                throw new ArgumentOutOfRangeException($"Position Z: {vector.Z} is invalid for grid!");

            return (gridX, gridZ);
        }

        private void ActivateGrid(int gridX, int gridZ)
        {
            for (int z = gridZ - GridBounds; z < gridZ + GridBounds; z++)
            {
                for (int x = gridX - GridBounds; x < gridX + GridBounds; x++)
                {
                    if (x < 0 || x > GridCount || z < 0 || z > GridCount)
                        continue;

                    if (grids[z * GridCount + x] != null)
                        continue;

                    Vector3 gridVector = new Vector3((x - GridOrigin) * MapGrid.Size, 0f, (z - GridOrigin) * MapGrid.Size);
                    grids[z * GridCount + x] = new MapGrid(gridVector);
                }
            }
        }
        
        private void AddEntity(GridAction action)
        {
            Debug.Assert(action.Entity.Map == null);

            GetGrid(action.Position).AddEntity(action);

            uint guid = entityCounter.Dequeue();
            entities.Add(guid, action.Entity);
            action.Entity.OnAddToMap(this, guid, action.Position);

            log.Trace($"Added entity {action.Entity.Guid} to map {Entry.Id}.");

            if (action.Entity is Player player)
                OnAddToMap(player);
        }

        protected virtual void OnAddToMap(Player player)
        {
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

        private void RelocateEntity(GridAction action)
        {
            Debug.Assert(action.Entity.Map != null);

            MapGrid newGrid = GetGrid(action.Position);
            MapGrid oldGrid = GetGrid(action.Entity.Position);

            if (newGrid.Vector != oldGrid.Vector)
            {
                oldGrid.RemoveEntity(action.Entity);
                newGrid.AddEntity(action);
            }
            else
                oldGrid.RelocateEntity(action);

            action.Entity.OnRelocate(action.Position);
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
            for (float z = vector.Z - radius; z <= vector.Z + radius; z += MapCell.Size)
            {
                for (float x = vector.X - radius; x <= vector.X + radius; x += MapCell.Size)
                {
                    var searchVector = new Vector3(x, 0f, z);
                    GetGrid(searchVector).Search(searchVector, check, intersectedEntities);
                }
            }
        }

        public virtual void Update(double lastTick)
        {
            while (pendingAdd.TryDequeue(out GridAction action))
                AddEntity(action);

            // relocate must be before remove to prevent relocating entities no longer in the grid
            while (pendingRelocate.TryDequeue(out GridAction action))
                RelocateEntity(action);

            while (pendingRemove.TryDequeue(out GridEntity entity))
                RemoveEntity(entity);

            foreach (MapGrid grid in grids.Where(g => g != null))
                grid.Update(lastTick);
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
    }
}
