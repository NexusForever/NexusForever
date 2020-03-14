using System.Numerics;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Game.Map.Search
{
    public class SearchCheckRangePlayerOnly : SearchCheckRange
    {
        public SearchCheckRangePlayerOnly(Vector3 vector, float radius, GridEntity exclude = null)
            : base(vector, radius, exclude)
        {
        }

        public override bool CheckEntity(GridEntity entity)
        {
            return entity is Player && base.CheckEntity(entity);
        }
    }
}
