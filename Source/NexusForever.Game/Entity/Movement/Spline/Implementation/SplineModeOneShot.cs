using NexusForever.Game.Static.Entity.Movement.Spline;

namespace NexusForever.Game.Entity.Movement.Spline.Implementation
{
    [SplineMode(SplineMode.OneShot)]
    public class SplineModeOneShot : ISplineMode
    {
        public SplineDirection InitialDirection => SplineDirection.Forward;

        public (SplineDirection Direction, uint Point)? GetNextPoint(SplineTypeBase type, SplineDirection direction, uint point)
        {
            if (point == type.TopIndex)
                return null;

            return (direction, point + 1u);
        }
    }
}
