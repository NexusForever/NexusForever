using NexusForever.Database.World.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;

namespace NexusForever.Game.Map
{
    internal sealed class GridActionRespawn : IGridAction
    {
        public required IGridEntity Entity { get; init; }
        public required EntityModel Spawn { get; init; }
    }
}
