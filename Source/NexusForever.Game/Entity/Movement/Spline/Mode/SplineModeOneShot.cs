using NexusForever.Game.Abstract.Entity.Movement.Spline.Mode;
using NexusForever.Game.Static.Entity.Movement.Spline;

namespace NexusForever.Game.Entity.Movement.Spline.Mode
{
    public class SplineModeOneShot : ISplineMode
    {
        public virtual SplineMode Mode => SplineMode.OneShot;

        /// <summary>
        /// Calculate the interpolated offset for the supplied offset value.
        /// </summary>
        public ISplineModeInterpolatedOffset GetInterpolatedOffset(float offset)
        {
            return new SplineModeInterpolatedOffset
            {
                Offset    = Math.Clamp(offset, 0.0f, 1.0f),
                Direction = SplineDirection.Forward,
                Finalised = offset >= 1.0f
            };
        }
    }
}
