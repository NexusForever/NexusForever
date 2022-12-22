using NexusForever.Game.Static.Entity.Movement.Spline;

namespace NexusForever.Game.Entity.Movement.Spline.Implementation
{
    [SplineMode(SplineMode.OneShotReverse)]
    public class SplineModeOneShotReverse : ISplineMode
    {
        public SplineDirection InitialDirection => SplineDirection.Backward;

        public (SplineDirection Direction, uint Point)? GetNextPoint(SplineTypeBase type, SplineDirection direction, uint point)
        {
            if (point == type.TopReverseIndex)
                return null;

            return (direction, point - 1u);
        }
    }
}
