using System.Numerics;
using NexusForever.Game.Static.Entity.Movement.Spline;

namespace NexusForever.Game.Abstract.Entity.Movement.Spline.Type
{
    public interface ISplineType
    {
        uint BottomIndex { get; }
        uint TopIndex { get; }
        uint BottomReverseIndex { get; }
        uint TopReverseIndex { get; }

        void Initialise(uint points);

        /// <summary>
        /// Get the interpolated <see cref="Vector3"/> between supplied points in <see cref="SplineDirection"/> at normalised (0 - totalLength) position p.
        /// </summary>
        Vector3 GetInterpolatedPoint(SplineDirection direction, float p, List<ISplinePoint> points);

        /// <summary>
        /// Get the interpolated <see cref="Vector3"/> between supplied points at normalised (0-1) distance t.
        /// </summary>
        Vector3 GetInterpolatedPoint(float t, params ISplinePoint[] points);

        /// <summary>
        /// Calculate the length of each segment and returns the total length.
        /// </summary>
        float CalculateLengths(List<ISplinePoint> points);
    }
}
