using System.Collections.Concurrent;
using System.Diagnostics;
using System.Numerics;
using NexusForever.Database.World.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Abstract.Map.Search;
using NexusForever.Game.Configuration.Model;
using NexusForever.Game.Entity;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Map;
using NexusForever.GameTable.Model;
using NexusForever.IO.Map;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Static;
using NexusForever.Script;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Collection;
using NexusForever.Shared;
using NexusForever.Shared.Configuration;
using NLog;

namespace NexusForever.Game.Map
{
    public class BaseMap : IBaseMap
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public const float DefaultVisionRange = 128f;

        /// <summary>
        /// Distance between a <see cref="IPlayer"/> and a <see cref="IGridEntity"/> before the entity can be seen.
        /// </summary>
        public virtual float? VisionRange { get; protected set; } = DefaultVisionRange;

        public WorldEntry Entry { get; private set; }
        public MapFile File { get; private set; }

        private readonly IMapGrid[] grids = new MapGrid[MapDefines.WorldGridCount * MapDefines.WorldGridCount];
        private readonly HashSet<(uint GridX, uint GridZ)> activeGrids = new();

        protected readonly ConcurrentQueue<IGridAction> pendingActions = new();

        private readonly QueuedCounter entityCounter = new();
        protected readonly Dictionary<uint /*guid*/, IGridEntity> entities = new();
        private IEntityCache entityCache;

        protected IScriptCollection scriptCollection;

        /// <summary>
        /// Initialise <see cref="IBaseMap"/> with <see cref="WorldEntry"/>.
        /// </summary>
        public virtual void Initialise(WorldEntry entry)
        {
            Entry            = entry;
            File             = MapIOManager.Instance.GetBaseMap(Entry.AssetPath);
            entityCache      = EntityCacheManager.Instance.GetEntityCache((ushort)Entry.Id);

            scriptCollection = ScriptManager.Instance.InitialiseOwnedScripts<IBaseMap>(this, Entry.Id);
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public virtual void Update(double lastTick)
        {
            ProcessGridActions();
            UpdateGrids(lastTick);

            scriptCollection?.Invoke<IUpdate>(s => s.Update(lastTick));
        }

        private void ProcessGridActions()
        {
            var newActions = new List<IGridAction>();
            foreach (IGridAction action in pendingActions.Dequeue(
                SharedConfiguration.Instance.Get<MapConfig>().GridActionThreshold ?? 100u))
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
                            if (actionAdd.RequeueCount > (SharedConfiguration.Instance.Get<MapConfig>().GridActionMaxRetry ?? 5u))
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
            var unloadedGrids = new List<IMapGrid>();
            foreach ((uint gridX, uint gridZ) in activeGrids)
            {
                IMapGrid grid = GetGrid(gridX, gridZ);
                grid.Update(lastTick);

                if (grid.UnloadStatus == MapUnloadStatus.Complete)
                    unloadedGrids.Add(grid);
            }

            foreach (IMapGrid grid in unloadedGrids)
            {
                grids[grid.Coord.Z * MapDefines.WorldGridCount + grid.Coord.X] = null;
                activeGrids.Remove(grid.Coord);

                log.Trace($"Deactivated grid at X:{grid.Coord.X}, Z:{grid.Coord.Z}.");
            }
        }

        /// <summary>
        /// Enqueue <see cref="IGridEntity"/> to be added to <see cref="IBaseMap"/>.
        /// </summary>
        /// <remarks>
        /// Characters should not be added directly through this method.
        /// Use <see cref="IPlayer.TeleportTo(IMapPosition, TeleportReason)"/> instead.
        /// </remarks>
        public void EnqueueAdd(IGridEntity entity, IMapPosition position)
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
        /// Returns if <see cref="IGridEntity"/> can be added to <see cref="IBaseMap"/>.
        /// </summary>
        public virtual bool CanEnter(IGridEntity entity, IMapPosition position)
        {
            if (!IsValidPosition(position.Position))
                return false;

            return true;
        }

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can be added to <see cref="IBaseMap"/>.
        /// </summary>
        public virtual GenericError? CanEnter(IPlayer player, IMapPosition position)
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
        /// Enqueue <see cref="IGridEntity"/> to be removed from <see cref="IBaseMap"/>.
        /// </summary>
        public void EnqueueRemove(IGridEntity entity)
        {
            entity.OnEnqueueRemoveFromMap();
            pendingActions.Enqueue(new GridActionRemove
            {
                Entity = entity
            });
        }

        /// <summary>
        /// Enqueue <see cref="IGridEntity"/> to be relocated in <see cref="IBaseMap"/> to <see cref="Vector3"/>.
        /// </summary>
        public void EnqueueRelocate(IGridEntity entity, Vector3 position)
        {
            pendingActions.Enqueue(new GridActionRelocate
            {
                Entity = entity,
                Vector = position
            });
        }

        /// <summary>
        /// Return all <see cref="IGridEntity"/>'s from <see cref="Vector3"/> in range that satisfy <see cref="ISearchCheck{T}"/>.
        /// </summary>
        public IEnumerable<T> Search<T>(Vector3 vector, float? radius, ISearchCheck<T> check) where T : IGridEntity
        {
            // no radius is unlimited distance
            if (radius == null)
            {
                foreach (T entity in entities.Values.OfType<T>().Where(check.CheckEntity))
                    yield return entity;
                yield break;
            }

            for (float z = vector.Z - radius.Value; z < vector.Z + radius.Value + MapDefines.GridCellSize; z += MapDefines.GridCellSize)
            {
                for (float x = vector.X - radius.Value; x < vector.X + radius.Value + MapDefines.GridCellSize; x += MapDefines.GridCellSize)
                {
                    var searchVector = new Vector3(x, 0f, z);

                    // don't activate new grids during search
                    IMapGrid grid = GetGrid(searchVector);
                    if (grid == null)
                        continue;

                    foreach (T entity in grid.Search(searchVector, check))
                        yield return entity;
                }
            }
        }

        /// <summary>
        /// Return all <see cref="IMapGrid"/>'s from <see cref="Vector3"/> in range.
        /// </summary>
        public void GridSearch(Vector3 vector, float? radius, out List<IMapGrid> intersectedGrids)
        {
            // negative radius is unlimited distance
            if (radius == null)
            {
                intersectedGrids = GetActiveGrids().ToList();
                return;
            }

            intersectedGrids = new List<IMapGrid>();
            for (float z = vector.Z - radius.Value; z < vector.Z + radius.Value + MapDefines.GridSize; z += MapDefines.GridSize)
            {
                for (float x = vector.X - radius.Value; x < vector.X + radius.Value + MapDefines.GridSize; x += MapDefines.GridSize)
                {
                    IMapGrid grid = GetGrid(new Vector3(x, 0f, z));
                    if (grid != null)
                        intersectedGrids.Add(grid);
                }
            }
        }

        /// <summary>
        /// Return <see cref="IGridEntity"/> by guid.
        /// </summary>
        public T GetEntity<T>(uint guid) where T : IGridEntity
        {
            if (!entities.TryGetValue(guid, out IGridEntity entity))
                return default;
            return (T)entity;
        }

        /// <summary>
        /// Enqueue broadcast of <see cref="IWritable"/> to all <see cref="IPlayer"/>'s on the map.
        /// </summary>
        public void EnqueueToAll(IWritable message)
        {
            foreach (IGridEntity entity in entities.Values)
            {
                IPlayer player = entity as IPlayer;
                player?.Session.EnqueueMessageEncrypted(message);
            }
        }

        /// <summary>
        /// Notify <see cref="IMapGrid"/> at coordinates of the addition of new <see cref="IPlayer"/> that is in vision range.
        /// </summary>
        public void GridAddVisiblePlayer(uint gridX, uint gridZ)
        {
            IMapGrid grid = GetGrid(gridX, gridZ);
            grid.AddVisiblePlayer();
        }

        /// <summary>
        /// Notify <see cref="IMapGrid"/> at coordinates of the removal of an existing <see cref="IPlayer"/> that is no longer in vision range.
        /// </summary>
        public void GridRemoveVisiblePlayer(uint gridX, uint gridZ)
        {
            IMapGrid grid = GetGrid(gridX, gridZ);
            grid.RemoveVisiblePlayer();
        }

        private IMapGrid GetGrid(uint gridX, uint gridZ)
        {
            return grids[gridZ * MapDefines.WorldGridCount + gridX];
        }

        private IMapGrid GetGrid(Vector3 vector)
        {
            (uint gridX, uint gridZ) = MapGrid.GetGridCoord(vector);
            return GetGrid(gridX, gridZ);
        }

        protected IEnumerable<IMapGrid> GetActiveGrids()
        {
            return activeGrids
                .Select(g => GetGrid(g.GridX, g.GridZ));
        }

        /// <summary>
        /// Activate one or more <see cref="IMapGrid"/>'s at <see cref="Vector"/> depending on vision range of <see cref="IGridEntity"/>.
        /// </summary>
        private void ActivateGrid(IGridEntity entity, Vector3 vector)
        {
            float range = entity.ActivationRange;
            for (float z = vector.Z - range; z < vector.Z + range + MapDefines.GridSize; z += MapDefines.GridSize)
            {
                for (float x = vector.X - range; x < vector.X + range + MapDefines.GridSize; x += MapDefines.GridSize)
                {
                    (uint gridX, uint gridZ) = MapGrid.GetGridCoord(new Vector3(x, 0f, z));
                    IMapGrid grid = GetGrid(gridX, gridZ);
                    if (grid == null)
                        ActivateGrid(gridX, gridZ);
                }
            }
        }

        /// <summary>
        /// Activate single <see cref="IMapGrid"/> at position.
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
                IWorldEntity entity = EntityManager.Instance.NewEntity((EntityType)model.Type) ?? EntityManager.Instance.NewEntity(EntityType.Simple);
                entity.Initialise(model);

                var position = new MapPosition
                {
                    Position = new Vector3(model.X, model.Y, model.Z)
                };

                if (CanEnter(entity, position))
                    EnqueueAdd(entity, position);
            }
        }

        private bool CanAddEntity(IGridEntity entity, Vector3 vector)
        {
            ActivateGrid(entity, vector);

            // if the grid doesn't exist we can't add the new entity to it
            IMapGrid grid = GetGrid(vector);
            if (grid == null)
                return false;

            // if the grid is unloading we can't add the new entity to it
            // we will need to wait for the grid to fully unload
            if (grid.UnloadStatus.HasValue)
                return false;

            return true;
        }

        protected virtual void AddEntity(IGridEntity entity, Vector3 vector)
        {
            Debug.Assert(entity.Map == null);

            IMapGrid grid = GetGrid(vector);
            grid.AddEntity(entity, vector);

            uint guid = entityCounter.Dequeue();
            entities.Add(guid, entity);

            entity.OnAddToMap(this, guid, vector);
            scriptCollection?.Invoke<IMapScript>(s => s.OnAddToMap(entity));

            log.Trace($"Added entity {entity.Guid} to map {Entry.Id} at {vector.X},{vector.Y},{vector.Z}.");
        }

        protected virtual void RemoveEntity(IGridEntity entity)
        {
            Debug.Assert(entity.Map != null);

            GetGrid(entity.Position).RemoveEntity(entity);

            log.Trace($"Removed entity {entity.Guid} from map {Entry.Id}.");

            entityCounter.Enqueue(entity.Guid);
            entities.Remove(entity.Guid);

            scriptCollection?.Invoke<IMapScript>(s => s.OnRemoveFromMap(entity));
            entity.OnRemoveFromMap();
        }

        protected virtual void RelocateEntity(IGridEntity entity, Vector3 vector)
        {
            Debug.Assert(entity.Map != null);

            ActivateGrid(entity, vector);
            IMapGrid newGrid = GetGrid(vector);
            IMapGrid oldGrid = GetGrid(entity.Position);

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
        public float? GetTerrainHeight(float x, float z)
        {
            // TODO: handle cases for water and props
            return File.GetTerrainHeight(new Vector3(x, 0, z));
        }

        /// <summary>
        /// Return <see cref="ResurrectionType"/> applicable to this map.
        /// </summary>
        public virtual ResurrectionType GetResurrectionType()
        {
            // TODO: add support for Holocrypts and instances
            return ResurrectionType.None;
        }
    }
}
