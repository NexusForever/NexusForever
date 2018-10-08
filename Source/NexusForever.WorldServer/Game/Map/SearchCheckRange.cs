using System.Numerics;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Game.Map
{
    public class SearchCheckRange : ISearchCheck
    {
        private readonly Vector3 vector;
        private readonly float radius;

        public SearchCheckRange(Vector3 vector, float radius)
        {
            this.vector = vector;
            this.radius = radius;
        }

        public bool CheckEntity(GridEntity entity)
        {
            return Vector3.Distance(vector, entity.Position) < radius;
        }
    }
}
