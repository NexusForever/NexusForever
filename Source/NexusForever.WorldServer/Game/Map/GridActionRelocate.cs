using System.Numerics;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Game.Map
{
    public class GridActionRelocate : IGridAction
    {
        public GridEntity Entity { get; init; }
        public Vector3 Vector { get; init; }
    }
}
