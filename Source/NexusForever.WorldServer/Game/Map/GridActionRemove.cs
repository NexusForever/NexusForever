using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Game.Map
{
    public class GridActionRemove : IGridAction
    {
        public GridEntity Entity { get; }
        public bool DropUnitId { get; private set; } = true;

        public GridActionRemove(GridEntity entity)
        {
            Entity = entity;
            if (entity is Player player && player.IsTeleporting())
                DropUnitId = false;
        }
    }
}
