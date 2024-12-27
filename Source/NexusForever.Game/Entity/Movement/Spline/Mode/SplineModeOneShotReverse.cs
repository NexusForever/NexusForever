using NexusForever.Game.Abstract.Entity.Movement.Spline.Mode;
using NexusForever.Game.Static.Entity.Movement.Spline;

namespace NexusForever.Game.Entity.Movement.Spline.Mode
{
    public class SplineModeOneShotReverse : ISplineMode
    {
        public SplineMode Mode => SplineMode.OneShotReverse;

        /// <summary>
        /// Calculate the interpolated offset for the supplied offset value.
        /// </summary>
        public ISplineModeInterpolatedOffset GetInterpolatedOffset(float offset)
        {
            return new SplineModeInterpolatedOffset
            {
                Offset    = 1.0f - Math.Clamp(offset, 0.0f, 1.0f),
                Direction = SplineDirection.Backward,
                Finalised = offset >= 1.0f
            };
        }
    }
}
