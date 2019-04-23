using NexusForever.WorldServer.Game.Entity.Movement.Spline.Static;

namespace NexusForever.WorldServer.Game.Entity.Movement.Spline.Implementation
{
    [SplineMode(SplineMode.CyclicReverse)]
    public class SplineModeCyclicReverse : ISplineMode
    {
        public SplineDirection InitialDirection => SplineDirection.Backward;

        public (SplineDirection Direction, uint Point)? GetNextPoint(SplineTypeBase type, SplineDirection direction, uint point)
        {
            if (point == type.TopReverseIndex)
                return (direction, type.BottomReverseIndex);

            return (direction, point - 1u);
        }
    }
}
