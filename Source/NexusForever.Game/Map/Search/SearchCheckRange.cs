using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map.Search;

namespace NexusForever.Game.Map.Search
{
    public class SearchCheckRange<T> : ISearchCheckRange<T> where T : IGridEntity
    {
        private Vector3 vector;
        private float? radius;

        public void Initialise(Vector3 vector, float? radius)
        {
            this.vector  = vector;
            this.radius   = radius;
        }

        public virtual bool CheckEntity(T entity)
        {
            return radius == null || Vector3.Distance(vector, entity.Position) < radius;
        }
    }
}
