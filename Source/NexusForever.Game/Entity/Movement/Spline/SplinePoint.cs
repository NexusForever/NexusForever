using System.Numerics;
using NexusForever.Game.Abstract.Entity.Movement.Spline;

namespace NexusForever.Game.Entity.Movement.Spline
{
    public class SplinePoint : ISplinePoint
    {
        public Vector3 Position { get; }
        public float Length { get; set; }

        public SplinePoint(Vector3 position)
        {
            Position = position;
        }
    }
}
