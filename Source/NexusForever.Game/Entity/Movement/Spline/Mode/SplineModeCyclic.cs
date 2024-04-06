using NexusForever.Game.Abstract.Entity.Movement.Spline.Mode;
using NexusForever.Game.Static.Entity.Movement.Spline;

namespace NexusForever.Game.Entity.Movement.Spline.Mode
{
    public class SplineModeCyclic : ISplineMode
    {
        public virtual SplineMode Mode => SplineMode.Cyclic;

        /// <summary>
        /// Calculate the interpolated offset for the supplied offset value.
        /// </summary>
        public ISplineModeInterpolatedOffset GetInterpolatedOffset(float offset)
        {
            return new SplineModeInterpolatedOffset
            {
                Offset    = offset % 1.0f,
                Direction = SplineDirection.Forward
            };
        }
    }
}
