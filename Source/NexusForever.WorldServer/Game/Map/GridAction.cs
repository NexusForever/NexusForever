using System.Numerics;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Game.Map
{
    public class GridAction
    {
        public GridEntity Entity { get; }
        public Vector3 Vector { get; }

        public GridAction(GridEntity entity, Vector3 vector)
        {
            Entity = entity;
            Vector = vector;
        }
    }
}
