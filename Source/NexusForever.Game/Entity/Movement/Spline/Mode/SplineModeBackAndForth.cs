using NexusForever.Game.Abstract.Entity.Movement.Spline.Mode;
using NexusForever.Game.Static.Entity.Movement.Spline;

namespace NexusForever.Game.Entity.Movement.Spline.Mode
{
    public class SplineModeBackAndForth : ISplineMode
    {
        public virtual SplineMode Mode => SplineMode.BackAndForth;

        /// <summary>
        /// Calculate the interpolated offset for the supplied offset value.
        /// </summary>
        public virtual ISplineModeInterpolatedOffset GetInterpolatedOffset(float offset)
        {
            offset %= 2.0f;
            return new SplineModeInterpolatedOffset
            {
                Offset    = offset > 1.0f ? 2.0f - offset : offset,
                Direction = offset > 1.0f ? SplineDirection.Backward : SplineDirection.Forward
            };
        }
    }
}
