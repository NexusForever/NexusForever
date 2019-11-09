using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using NexusForever.Shared;
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

        /// <summary>
        /// Distance between a <see cref="GridEntity"/> and a <see cref="MapGrid"/> for activation.
        /// </summary>
        public float ActivationRange { get; protected set; }

        protected readonly Dictionary<uint, GridEntity> visibleEntities = new Dictionary<uint, GridEntity>();

        /// <summary>
        /// Enqueue for removal from the map.
        /// </summary>
        public void RemoveFromMap()
        {
            Debug.Assert(Map != null);
            Map.EnqueueRemove(this);
        }

        /// <summary>
        /// Enqueue for relocation on the map.
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

            Guid = 0;
            Map  = null;
        }

        /// <summary>
        /// Invoked when <see cref="GridEntity"/> is relocated.
        /// </summary>
        public virtual void OnRelocate(Vector3 vector)
        {
            Position = vector;
            UpdateVision();

            uint worldAreaId = Map.File.GetWorldAreaId(vector);
            if (Zone?.Id != worldAreaId)
            {
                Zone = GameTableManager.Instance.WorldZone.GetEntry(worldAreaId);
                OnZoneUpdate();
            }
        }

        protected virtual void OnZoneUpdate()
        {
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
        public void UpdateVision()
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

        public abstract void Update(double lastTick);
    }
}
