using System.Numerics;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Game.Map
{
    public class GridActionAdd : IGridAction
    {
        public uint RequeueCount { get; set; }

        public GridEntity Entity { get; }
        public Vector3 Vector { get; }

        public GridActionAdd(GridEntity entity, Vector3 vector)
        {
            Entity = entity;
            Vector = vector;
        }
    }
}
