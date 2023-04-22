using NexusForever.Game.Abstract.Entity.Movement.Spline;
using NexusForever.Game.Abstract.Entity.Movement.Spline.Type;
using NexusForever.Game.Static.Entity.Movement.Spline;

namespace NexusForever.Game.Entity.Movement.Spline.Mode
{
    [SplineMode(SplineMode.OneShotReverse)]
    public class SplineModeOneShotReverse : ISplineMode
    {
        public SplineDirection InitialDirection => SplineDirection.Backward;

        public (SplineDirection Direction, uint Point)? GetNextPoint(ISplineType type, SplineDirection direction, uint point)
        {
            if (point == type.TopReverseIndex)
                return null;

            return (direction, point - 1u);
        }
    }
}
