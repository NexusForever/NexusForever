using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Game.Map
{
    public class GridActionRemove : IGridAction
    {
        public GridEntity Entity { get; }

        public GridActionRemove(GridEntity entity)
        {
            Entity = entity;
        }
    }
}
