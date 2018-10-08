using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NexusForever.WorldServer.Game.Entity;
using NLog;

namespace NexusForever.WorldServer.Game.Map
{
    public class MapCell
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Size of cell, represents 32 world units on all sides.
        /// </summary>
        public const int Size = 32;

        public Vector3 Vector { get; }

        private readonly HashSet<GridEntity> entities = new HashSet<GridEntity>();

        /// <summary>
        /// Initialise <see cref="MapCell"/> at the specified world position.
        /// </summary>
        public MapCell(Vector3 vector)
        {
            Vector = vector;
        }

        public void AddEntity(GridEntity entity)
        {
            entities.Add(entity);
            log.Trace($"Added entity {entity.Guid} to cell at X:{Vector.X}, Z:{Vector.Z}.");
        }

        public void RemoveEntity(GridEntity entity)
        {
            entities.Remove(entity);
            log.Trace($"Removed entity {entity.Guid} from cell at X:{Vector.X}, Z:{Vector.Z}.");
        }

        /// <summary>
        /// Return all <see cref="GridEntity"/>'s in cell that satisfy <see cref="ISearchCheck"/>.
        /// </summary>
        public void Search(ISearchCheck check, List<GridEntity> intersectedEntities)
        {
            intersectedEntities.AddRange(entities.Where(check.CheckEntity));
        }

        public void Update(double lastTick)
        {
            foreach (GridEntity entity in entities)
                entity.Update(lastTick);
        }
    }
}
