using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map.Search;

namespace NexusForever.Game.Map.Search
{
    public class SearchCheckRange<T> : ISearchCheck<T> where T : class, IGridEntity
    {
        private readonly Vector3 vector;
        private readonly float? radius;
        private readonly T exclude;

        public SearchCheckRange(Vector3 vector, float? radius, T exclude = null)
        {
            this.vector  = vector;
            this.radius  = radius;
            this.exclude = exclude;
        }

        public virtual bool CheckEntity(T entity)
        {
            if (exclude != null && entity == exclude)
                return false;

            return radius == null || Vector3.Distance(vector, entity.Position) < radius;
        }
    }
}
