using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using NexusForever.WorldServer.Game.Map;

namespace NexusForever.WorldServer.Game.Entity
{
    public abstract class GridEntity
    {
        // arbitrary
        private const float VisionRange = 32.0f;

        public uint Guid { get; protected set; }
        public BaseMap Map { get; private set; }
        public Vector3 Position { get; protected set; }

        protected readonly HashSet<GridEntity> visibleEntities = new HashSet<GridEntity>();

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

        public virtual void OnAddToMap(BaseMap map, uint guid, Vector3 vector)
        {
            Guid   = guid;
            Map    = map;
            Position = vector;
            UpdateVision();
        }

        public virtual void OnRemoveFromMap()
        {
            foreach (GridEntity entity in visibleEntities.ToList())
                entity.RemoveVisible(this);

            visibleEntities.Clear();

            Guid = 0;
            Map  = null;
        }

        public virtual void OnRelocate(Vector3 vector)
        {
            Position = vector;
            UpdateVision();
        }

        /// <summary>
        /// Add tracked <see cref="GridEntity"/> that is in vision range.
        /// </summary>
        public virtual void AddVisible(GridEntity entity)
        {
            /*if (!CanSeeEntity(entity))
                return;*/

            visibleEntities.Add(entity);
        }

        /// <summary>
        /// Remove tracked <see cref="GridEntity"/> that is no longer in vision range.
        /// </summary>
        public virtual void RemoveVisible(GridEntity entity)
        {
            visibleEntities.Remove(entity);
        }

        /// <summary>
        /// Update all <see cref="GridEntity"/>'s in vision range.
        /// </summary>
        public void UpdateVision()
        {
            Map.Search(Position, VisionRange, new SearchCheckRange(Position, VisionRange), out List<GridEntity> intersectedEntities);

            // new entities now in vision range
            foreach (GridEntity entity in intersectedEntities.Except(visibleEntities))
            {
                AddVisible(entity);
                if (entity != this)
                    entity.AddVisible(this);
            }

            // old entities now out of vision range
            foreach (GridEntity entity in visibleEntities.Except(intersectedEntities).ToList())
            {
                RemoveVisible(entity);
                entity.RemoveVisible(this);
            }
        }

        public abstract void Update(double lastTick);
    }
}
