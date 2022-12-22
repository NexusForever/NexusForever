using NexusForever.Game.Static.Entity.Movement.Spline;

namespace NexusForever.Game.Entity.Movement.Spline.Implementation
{
    [SplineMode(SplineMode.BackAndForthReverse)]
    public class SplineModeBackAndForthReverse : SplineModeBackAndForth
    {
        public override SplineDirection InitialDirection => SplineDirection.Backward;
    }
}
