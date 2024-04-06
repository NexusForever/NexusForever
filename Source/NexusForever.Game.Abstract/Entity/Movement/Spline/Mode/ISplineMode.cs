using NexusForever.Game.Static.Entity.Movement.Spline;

namespace NexusForever.Game.Abstract.Entity.Movement.Spline.Mode
{
    public interface ISplineMode
    {
        SplineMode Mode { get; }

        /// <summary>
        /// Calculate the interpolated offset for the supplied offset value.
        /// </summary>
        ISplineModeInterpolatedOffset GetInterpolatedOffset(float offset);
    }
}
