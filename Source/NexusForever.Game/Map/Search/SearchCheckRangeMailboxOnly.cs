using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Entity;

namespace NexusForever.Game.Map.Search
{
    public class SearchCheckRangeMailboxOnly : SearchCheckRange
    {
        public SearchCheckRangeMailboxOnly(Vector3 vector, float radius, IGridEntity exclude = null)
            : base(vector, radius, exclude)
        {
        }

        public override bool CheckEntity(IGridEntity entity)
        {
            return entity is Mailbox && base.CheckEntity(entity);
        }
    }
}
