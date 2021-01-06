using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using NexusForever.Shared;
using NexusForever.Shared.Game.Map;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Map;
using NexusForever.WorldServer.Game.Map.Search;

namespace NexusForever.WorldServer.Game.Entity
{
    public abstract class GridEntity : IUpdate
    {
        public uint Guid { get; protected set; }
        public BaseMap Map { get; private set; }
        public WorldZoneEntry Zone { get; private set; }
        public Vector3 Position { get; protected set; }

        public MapInfo PreviousMap { get; private set; }

        /// <summary>
        /// Distance between a <see cref="GridEntity"/> and a <see cref="MapGrid"/> for activation.
        /// </summary>
        public float ActivationRange { get; protected set; }

        protected readonly Dictionary<uint, GridEntity> visibleEntities = new Dictionary<uint, GridEntity>();

        private readonly HashSet<(uint GridX, uint GridZ)> visibleGrids = new HashSet<(uint GridX, uint GridZ)>();

        /// <summary>
        /// Enqueue  <see cref="GridEntity"/> for removal from the <see cref="BaseMap"/>.
        /// </summary>
        public void RemoveFromMap()
        {
            Debug.Assert(Map != null);
            Map.EnqueueRemove(this);
        }

        /// <summary>
        /// Remove <see cref="GridEntity"/> from the <see cref="BaseMap"/>.
        /// </summary>
        /// <remarks>
        /// This will remove the entity right away, this should only be used in a few cases such as <see cref="MapGrid"/> unloading.
        /// </remarks>
        public void RemoveFromMapDirect()
        {
            Debug.Assert(Map != null);
            Map.RemoveDirect(this);
        }

        /// <summary>
        /// Enqueue  <see cref="GridEntity"/> for relocation on the <see cref="BaseMap"/>.
        /// </summary>
        public void Relocate(Vector3 position)
        {
            Debug.Assert(Map != null);
            Map.EnqueueRelocate(this, position);
        }

        /// <summary>
        /// Invoked when <see cref="GridEntity"/> is enqueued to be added to <see cref="BaseMap"/>.
        /// </summary>
        public virtual void OnEnqueueAddToMap()
        {
            // deliberately empty
        }

        /// <summary>
        /// Invoked when <see cref="GridEntity"/> is added to <see cref="BaseMap"/>.
        /// </summary>
        public virtual void OnAddToMap(BaseMap map, uint guid, Vector3 vector)
        {
            Guid     = guid;
            Map      = map;
            Position = vector;

            UpdateVision();
            UpdateGridVision();
        }

        /// <summary>
        /// Invoked when <see cref="GridEntity"/> is enqueued to be removed from <see cref="BaseMap"/>.
        /// </summary>
        public virtual void OnEnqueueRemoveFromMap()
        {
            // deliberately empty
        }

        /// <summary>
        /// Invoked when <see cref="GridEntity"/> is removed from <see cref="BaseMap"/>.
        /// </summary>
        public virtual void OnRemoveFromMap()
        {
            foreach (GridEntity entity in visibleEntities.Values.ToList())
                entity.RemoveVisible(this);

            visibleEntities.Clear();

            foreach ((uint gridX, uint gridZ) in visibleGrids.ToList())
                RemoveVisible(gridX, gridZ);

            visibleGrids.Clear();

            Guid        = 0;
            PreviousMap = new MapInfo(Map.Entry);
            Map         = null;
        }

        /// <summary>
        /// Invoked when <see cref="GridEntity"/> is relocated.
        /// </summary>
        public virtual void OnRelocate(Vector3 vector)
        {
            Position = vector;
            UpdateVision();
            UpdateGridVision();

            uint worldAreaId = Map.File.GetWorldAreaId(vector);
            if (Zone?.Id != worldAreaId)
            {
                Zone = GameTableManager.Instance.WorldZone.GetEntry(worldAreaId);
                OnZoneUpdate();
            }
        }

        /// <summary>
        /// Invoked when <see cref="GridEntity"/> changes zone in the current <see cref="BaseMap"/>.
        /// </summary>
        protected virtual void OnZoneUpdate()
        {
            // deliberately empty
        }

        /// <summary>
        /// Returns if <see cref="GridEntity"/> can see supplied <see cref="GridEntity"/>.
        /// </summary>
        public virtual bool CanSeeEntity(GridEntity entity)
        {
            return true;
        }

        /// <summary>
        /// Add tracked <see cref="GridEntity"/> that is in vision range.
        /// </summary>
        public virtual void AddVisible(GridEntity entity)
        {
            if (!CanSeeEntity(entity))
                return;

            visibleEntities.Add(entity.Guid, entity);
        }

        /// <summary>
        /// Remove tracked <see cref="GridEntity"/> that is no longer in vision range.
        /// </summary>
        public virtual void RemoveVisible(GridEntity entity)
        {
            visibleEntities.Remove(entity.Guid);
        }

        /// <summary>
        /// Return visible <see cref="WorldEntity"/> by supplied guid.
        /// </summary>
        public T GetVisible<T>(uint guid) where T : WorldEntity
        {
            if (!visibleEntities.TryGetValue(guid, out GridEntity entity))
                return null;
            return (T)entity;
        }

        /// <summary>
        /// Return visible <see cref="WorldEntity"/> by supplied creature id.
        /// </summary>
        public IEnumerable<T> GetVisibleCreature<T>(uint creatureId) where T : WorldEntity
        {
            // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
            foreach (WorldEntity entity in visibleEntities.Values)
                if (entity.CreatureId == creatureId)
                    yield return (T)entity;
        }

        /// <summary>
        /// Update all <see cref="GridEntity"/>'s in vision range.
        /// </summary>
        private void UpdateVision()
        {
            Map.Search(Position, Map.VisionRange, new SearchCheckRange(Position, Map.VisionRange), out List<GridEntity> intersectedEntities);

            // new entities now in vision range
            foreach (GridEntity entity in intersectedEntities.Except(visibleEntities.Values))
            {
                AddVisible(entity);
                if (entity != this)
                    entity.AddVisible(this);
            }

            // old entities now out of vision range
            foreach (GridEntity entity in visibleEntities.Values.Except(intersectedEntities).ToList())
            {
                RemoveVisible(entity);
                entity.RemoveVisible(this);
            }
        }

        /// <summary>
        /// Search for all <see cref="MapGrid"/>'s the <see cref="GridEntity"/> can see with the current <see cref="BaseMap"/> vision range.
        /// </summary>
        private void GridSearch(Vector3 vector, List<(uint GridX, uint GridZ)> intersectedGrids)
        {
            for (float z = vector.Z - Map.VisionRange; z < vector.Z + Map.VisionRange + MapDefines.GridSize; z += MapDefines.GridSize)
                for (float x = vector.X - Map.VisionRange; x < vector.X + Map.VisionRange + MapDefines.GridSize; x += MapDefines.GridSize)
                    intersectedGrids.Add(MapGrid.GetGridCoord(new Vector3(x, 0f, z)));
        }

        /// <summary>
        /// Update all <see cref="MapGrid"/>'s in vision range.
        /// </summary>
        private void UpdateGridVision()
        {
            var intersectedGrids = new List<(uint GridX, uint GridZ)>();
            GridSearch(Position, intersectedGrids);

            // new grids now in vision range
            foreach ((uint gridX, uint gridZ) in intersectedGrids.Except(visibleGrids))
            {
                visibleGrids.Add((gridX, gridZ));
                if (this is Player)
                    Map.GridAddVisiblePlayer(gridX, gridZ);
            }

            // old grids now out of vision range
            foreach ((uint gridX, uint gridZ) in visibleGrids.Except(intersectedGrids).ToList())
                RemoveVisible(gridX, gridZ);
        }

        private void RemoveVisible(uint gridX, uint gridZ)
        {
            visibleGrids.Remove((gridX, gridZ));
            if (this is Player)
                Map.GridRemoveVisiblePlayer(gridX, gridZ);
        }

        public abstract void Update(double lastTick);
    }
}
