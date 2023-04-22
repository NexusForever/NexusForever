using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Entity;

namespace NexusForever.Game.Map.Search
{
    public class SearchCheckRangeDoorOnly : SearchCheckRange
    {
        public SearchCheckRangeDoorOnly(Vector3 vector, float radius, IGridEntity exclude = null)
            : base(vector, radius, exclude)
        {
        }

        public override bool CheckEntity(IGridEntity entity)
        {
            return entity is Door && base.CheckEntity(entity);
        }
    }
}
