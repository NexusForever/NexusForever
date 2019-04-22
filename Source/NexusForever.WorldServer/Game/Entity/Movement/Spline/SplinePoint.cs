using System.Numerics;

namespace NexusForever.WorldServer.Game.Entity.Movement.Spline
{
    public class SplinePoint
    {
        public Vector3 Position { get; }
        public float Length { get; set; }

        public SplinePoint(Vector3 position)
        {
            Position = position;
        }
    }
}
