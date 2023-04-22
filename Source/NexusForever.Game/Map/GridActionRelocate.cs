using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;

namespace NexusForever.Game.Map
{
    public class GridActionRelocate : IGridActionRelocate
    {
        public IGridEntity Entity { get; init; }
        public Vector3 Vector { get; init; }
    }
}
