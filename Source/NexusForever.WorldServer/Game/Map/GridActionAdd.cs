using System.Numerics;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Game.Map
{
    public class GridActionAdd : IGridAction
    {
        public GridEntity Entity { get; init; }
        public Vector3 Vector { get; init; }
        public uint RequeueCount { get; set; }
    }
}
