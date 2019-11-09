using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using NexusForever.Shared.Configuration;
using NexusForever.Shared.Game.Map;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.IO.Map;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Database.World;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Map.Search;
using NLog;
using EntityModel = NexusForever.WorldServer.Database.World.Model.Entity;
using Path = System.IO.Path;

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

        private readonly ConcurrentQueue<GridAction> pendingAdd = new ConcurrentQueue<GridAction>();
        private readonly ConcurrentQueue<GridEntity> pendingRemove = new ConcurrentQueue<GridEntity>();
        private readonly ConcurrentQueue<GridAction> pendingRelocate = new ConcurrentQueue<GridAction>();

        private readonly QueuedCounter entityCounter = new QueuedCounter();
        private readonly Dictionary<uint /*guid*/, GridEntity> entities = new Dictionary<uint /*guid*/, GridEntity>();
        private readonly EntityCache entityCache = new EntityCache();

        /// <summary>
        /// Returns a <see cref="MapFile"/> for the supplied asset.
        /// </summary>
        public static MapFile LoadMapFile(string assetPath)
        {
            string mapPath  = ConfigurationManager<WorldServerConfiguration>.Instance.Config.Map.MapPath;
            string asset    = Path.Combine(mapPath, Path.GetFileName(assetPath));
            string filePath = Path.ChangeExtension(asset, ".nfmap");

            using (var stream = System.IO.File.OpenRead(filePath))
            using (var reader = new BinaryReader(stream))
            {
                var mapFile = new MapFile();
                mapFile.Read(reader);
                return mapFile;
            }
        }

        public virtual void Initialise(MapInfo info, Player player)
        {
            Entry      = info.Entry;
            File       = LoadMapFile(Entry.AssetPath);
            InstanceId = info.InstanceId;
           
            CacheEntitySpawns();
        }
        
        private void CacheEntitySpawns()
        {
            uint count = 0u;
            foreach (EntityModel model in WorldDatabase.GetEntities((ushort)Entry.Id))
            {
                entityCache.AddEntity(model);
                count++;
            }

            log.Trace($"Initialised {count} spawns for world {Entry.Id}.");
        }

        public virtual void Update(double lastTick)
        {
            while (pendingAdd.TryDequeue(out GridAction action))
                AddEntity(action.Entity, action.Vector);

            // relocate must be before remove to prevent relocating entities no longer in the grid
            while (pendingRelocate.TryDequeue(out GridAction action))
                RelocateEntity(action.Entity, action.Vector);

            while (pendingRemove.TryDequeue(out GridEntity entity))
                RemoveEntity(entity);

            foreach (MapGrid grid in grids.Where(g => g != null))
                grid.Update(lastTick);
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
            pendingAdd.Enqueue(new GridAction(entity, position));
        }

        /// <summary>
        /// Enqueue <see cref="GridEntity"/> to be removed from <see cref="BaseMap"/>.
        /// </summary>
        public void EnqueueRemove(GridEntity entity)
        {
            entity.OnEnqueueRemoveFromMap();
            pendingRemove.Enqueue(entity);
        }

        /// <summary>
        /// Enqueue <see cref="GridEntity"/> to be relocated in <see cref="BaseMap"/> to <see cref="Vector3"/>.
        /// </summary>
        public void EnqueueRelocate(GridEntity entity, Vector3 position)
        {
            pendingRelocate.Enqueue(new GridAction(entity, position));
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

            log.Trace($"Activated grid at X:{gridX}, Z:{gridZ}.");

            foreach (EntityModel model in entityCache.GetEntities(gridX, gridZ))
            {
                // non issue once all entities types are handled
                WorldEntity entity = EntityManager.Instance.NewEntity((EntityType)model.Type) ?? EntityManager.Instance.NewEntity(EntityType.Simple);
                entity.Initialise(model);

                var vector = new Vector3(model.X, model.Y, model.Z);
                AddEntity(grid, entity, vector);
            }
        }

        private void AddEntity(GridEntity entity, Vector3 vector)
        {
            Debug.Assert(entity.Map == null);

            ActivateGrid(entity, vector);
            AddEntity(GetGrid(vector), entity, vector);
        }

        private void AddEntity(MapGrid grid, GridEntity entity, Vector3 vector)
        {
            Debug.Assert(entity.Map == null);

            grid.AddEntity(entity, vector);

            uint guid = entityCounter.Dequeue();
            entities.Add(guid, entity);
            entity.OnAddToMap(this, guid, vector);

            log.Trace($"Added entity {entity.Guid} to map {Entry.Id}.");
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
