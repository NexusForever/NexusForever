using System.Numerics;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Game.Map
{
    public class GridActionPending : IGridAction
    {
        public GridEntity Entity { get; init; }
        public Vector3 Vector { get; init; }
    }
}
