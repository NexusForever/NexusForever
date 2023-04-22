using System.Numerics;
using NexusForever.Game.Abstract.Entity.Movement.Spline.Type;
using NexusForever.Game.Static.Entity.Movement.Spline;

namespace NexusForever.Game.Abstract.Entity.Movement.Spline
{
    public interface ISpline : IEnumerable<ISplinePoint>
    {
        float Length { get; }

        /// <summary>
        /// Initialise a new single spline with supplied <see cref="ISplineType"/> and <see cref="ISplineMode"/>.
        /// </summary>
        void Initialise(ushort splineId, ISplineType splineType, ISplineMode splineMode);

        /// <summary>
        /// Initialise a new custom spline with supplied <see cref="ISplineType"/> and <see cref="ISplineMode"/>.
        /// </summary>
        void Initialise(List<Vector3> nodes, ISplineType splineType, ISplineMode splineMode);

        /// <summary>
        /// Get the final point position in the supplied <see cref="SplineDirection"/>.
        /// </summary>
        Vector3 GetFinalPoint(SplineDirection direction);

        /// <summary>
        /// Get the <see cref="SplineDirection"/> and index of the next point from the specified <see cref="SplineDirection"/> and index.
        /// </summary>
        (SplineDirection Direction, uint Point)? GetNextPoint(SplineDirection direction, uint point);

        /// <summary>
        /// Get the length of the next segment from the specified <see cref="SplineDirection"/> and index.
        /// </summary>
        float GetNextLength(SplineDirection direction, uint point);

        /// <summary>
        /// Get the interpolated <see cref="Vector3"/> position in the specified <see cref="SplineDirection"/> at p.
        /// </summary>
        Vector3 GetPosition(SplineDirection direction, float p);
    }
}