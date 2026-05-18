using System.Collections.Immutable;
using System.Numerics;
using NexusForever.Database.World.Model;
using NexusForever.Game.Abstract.Map;

namespace NexusForever.Game.Map
{
    public class EntityCache : IEntityCache
    {
        public uint GridCount => (uint)entities.Count;
        public uint EntityCount { get; }

        private readonly Dictionary<(uint GridX, uint GridZ), HashSet<EntityModel>> entities = new();

        public EntityCache(ImmutableList<EntityModel> models)
        {
            EntityCount = (uint)models.Count;
            foreach (EntityModel model in models)
                AddEntity(model);
        }

        /// <summary>
        /// Add <see cref="EntityModel"/> to be spawned when parent <see cref="IMapGrid"/> is activated.
        /// </summary>
        public void AddEntity(EntityModel model)
        {
            var vector = new Vector3(model.X, model.Y, model.Z);
            (uint GridX, uint GridZ) coord = MapGrid.GetGridCoord(vector);

            if (!entities.ContainsKey(coord))
                entities.Add(coord, new HashSet<EntityModel>());

            entities[coord].Add(model);
        }

        /// <summary>
        /// Return all <see cref="EntityModel"/>'s to be spawned for parent <see cref="IMapGrid"/>.
        /// </summary>
        public IEnumerable<EntityModel> GetEntities(uint gridX, uint gridZ)
        {
            return entities.TryGetValue((gridX, gridZ), out HashSet<EntityModel> cellEntities) ? cellEntities : Enumerable.Empty<EntityModel>();
        }
    }
}
