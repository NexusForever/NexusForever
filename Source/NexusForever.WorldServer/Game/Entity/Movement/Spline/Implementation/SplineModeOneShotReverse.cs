using NexusForever.WorldServer.Game.Entity.Movement.Spline.Static;

namespace NexusForever.WorldServer.Game.Entity.Movement.Spline.Implementation
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
