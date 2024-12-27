using NexusForever.Game.Abstract.Entity.Movement.Spline.Mode;
using NexusForever.Game.Static.Entity.Movement.Spline;

namespace NexusForever.Game.Entity.Movement.Spline.Mode
{
    public class SplineModeBackAndForthReverse : ISplineMode
    {
        public SplineMode Mode => SplineMode.BackAndForthReverse;

        /// <summary>
        /// Calculate the interpolated offset for the supplied offset value.
        /// </summary>
        public ISplineModeInterpolatedOffset GetInterpolatedOffset(float offset)
        {
            offset %= 2.0f;
            return new SplineModeInterpolatedOffset
            {
                Offset    = offset > 1.0f ? 1.0f - (2.0f - offset) : 1.0f - offset,
                Direction = offset > 1.0f ? SplineDirection.Forward : SplineDirection.Backward
            };
        }
    }
}
