using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NexusForever.Database.World.Model;

namespace NexusForever.WorldServer.Game.Map
{
    public class EntityCache
    {
        private readonly Dictionary<ushort /*gridHash*/, HashSet<EntityModel>> entities = new Dictionary<ushort, HashSet<EntityModel>>();

        private static ushort GetGridHash(uint gridX, uint gridZ)
        {
            return (ushort)((gridZ << 8) | gridX);
        }

        /// <summary>
        /// Add <see cref="EntityModel"/> to be spawned when parent <see cref="MapGrid"/> is activated.
        /// </summary>
        public void AddEntity(EntityModel model)
        {
            var vector = new Vector3(model.X, model.Y, model.Z);
            (uint gridX, uint gridZ) = MapGrid.GetGridCoord(vector);

            ushort hash = GetGridHash(gridX, gridZ);
            if (!entities.ContainsKey(hash))
                entities.Add(hash, new HashSet<EntityModel>());

            entities[hash].Add(model);
        }

        /// <summary>
        /// Return all <see cref="EntityModel"/>'s to be spawned for parent <see cref="MapGrid"/>.
        /// </summary>
        public IEnumerable<EntityModel> GetEntities(uint gridX, uint gridZ)
        {
            ushort hash = GetGridHash(gridX, gridZ);
            return entities.TryGetValue(hash, out HashSet<EntityModel> cellEntities) ? cellEntities : Enumerable.Empty<EntityModel>();
        }
    }
}
