using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map.Search;

namespace NexusForever.Game.Map.Search
{
    public class SearchCheckRange : ISearchCheck
    {
        private readonly Vector3 vector;
        private readonly float radius;
        private readonly IGridEntity exclude;

        public SearchCheckRange(Vector3 vector, float radius, IGridEntity exclude = null)
        {
            this.vector  = vector;
            this.radius  = radius;
            this.exclude = exclude;
        }

        public virtual bool CheckEntity(IGridEntity entity)
        {
            if (exclude != null && entity == exclude)
                return false;

            return Vector3.Distance(vector, entity.Position) < radius;
        }
    }
}
