using System.Numerics;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Game.Map.Search
{
    public class SearchCheckRange : ISearchCheck
    {
        private readonly Vector3 vector;
        private readonly float radius;
        private readonly GridEntity exclude;

        public SearchCheckRange(Vector3 vector, float radius, GridEntity exclude = null)
        {
            this.vector  = vector;
            this.radius  = radius;
            this.exclude = exclude;
        }

        public virtual bool CheckEntity(GridEntity entity)
        {
            if (exclude != null && entity == exclude)
                return false;

            return Vector3.Distance(vector, entity.Position) < radius;
        }
    }
}
