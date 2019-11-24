using System.Numerics;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Game.Map
{
    public class GridActionRelocate : IGridAction
    {
        public GridEntity Entity { get; }
        public Vector3 Vector { get; }

        public GridActionRelocate(GridEntity entity, Vector3 vector)
        {
            Entity = entity;
            Vector = vector;
        }
    }
}
