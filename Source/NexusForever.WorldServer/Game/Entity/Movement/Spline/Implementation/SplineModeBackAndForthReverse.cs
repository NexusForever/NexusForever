using NexusForever.WorldServer.Game.Entity.Movement.Spline.Static;

namespace NexusForever.WorldServer.Game.Entity.Movement.Spline.Implementation
{
    [SplineMode(SplineMode.BackAndForthReverse)]
    public class SplineModeBackAndForthReverse : SplineModeBackAndForth
    {
        public override SplineDirection InitialDirection => SplineDirection.Backward;
    }
}
