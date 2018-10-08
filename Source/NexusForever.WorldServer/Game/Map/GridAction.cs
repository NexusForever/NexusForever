using System.Numerics;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Game.Map
{
    public class GridAction
    {
        public GridEntity Entity { get; }
        public Vector3 Position { get; }

        public GridAction(GridEntity entity, Vector3 position)
        {
            Entity   = entity;
            Position = position;
        }
    }
}
