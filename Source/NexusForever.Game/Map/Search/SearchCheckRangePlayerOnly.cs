using System.Numerics;
using NexusForever.Game.Abstract.Entity;

namespace NexusForever.Game.Map.Search
{
    public class SearchCheckRangePlayerOnly : SearchCheckRange
    {
        public SearchCheckRangePlayerOnly(Vector3 vector, float radius, IGridEntity exclude = null)
            : base(vector, radius, exclude)
        {
        }

        public override bool CheckEntity(IGridEntity entity)
        {
            return entity is IPlayer && base.CheckEntity(entity);
        }
    }
}
