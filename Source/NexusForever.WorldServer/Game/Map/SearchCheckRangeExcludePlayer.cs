using System.Numerics;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Game.Map
{
    public class SearchCheckRangeExcludeGridEntity : SearchCheckRange
    {
        private readonly GridEntity entity;

        public SearchCheckRangeExcludeGridEntity(Vector3 vector, float radius, GridEntity entity):base(vector, radius)
        {
            this.entity = entity;
        }

        public override bool CheckEntity(GridEntity entity)
        {
            return entity != this.entity && base.CheckEntity(entity);
        }
    }
}
