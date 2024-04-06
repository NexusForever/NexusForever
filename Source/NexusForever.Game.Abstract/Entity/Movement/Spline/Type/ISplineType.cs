using System.Numerics;
using NexusForever.Game.Static.Entity.Movement.Spline;

namespace NexusForever.Game.Abstract.Entity.Movement.Spline.Type
{
    public interface ISplineType
    {
        SplineType Type { get; }

        /// <summary>
        /// Total length of the spline.
        /// </summary>
        float Length { get; }

        /// <summary>
        /// Total length of delays in the spline.
        /// </summary>
        float DelayLength { get; }

        /// <summary>
        /// Initialise the spline with a collection of <see cref="ISplinePoint"/>.
        /// </summary>
        void Initialise(List<ISplinePoint> points);

        /// <summary>
        /// Get the interpolated position between two points.
        /// </summary>
        Vector3 GetInterpolatedPosition(ISplinePoint point, ISplinePoint point2, float t);

        /// <summary>
        /// Get the interpolated rotation between two points.
        /// </summary>
        Vector3 GetInterpolatedRotation(ISplinePoint point, ISplinePoint point2, float t);
    }
}
