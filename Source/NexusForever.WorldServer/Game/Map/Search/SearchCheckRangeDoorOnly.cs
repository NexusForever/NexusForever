using System.Numerics;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Game.Map.Search
{
    public class SearchCheckRangeDoorOnly : SearchCheckRange
    {
        public SearchCheckRangeDoorOnly(Vector3 vector, float radius, GridEntity exclude = null)
            : base(vector, radius, exclude)
        {
        }

        public override bool CheckEntity(GridEntity entity)
        {
            return entity is Door && base.CheckEntity(entity);
        }
    }
}
