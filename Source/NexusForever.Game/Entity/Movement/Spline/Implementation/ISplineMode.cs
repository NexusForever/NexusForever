using NexusForever.Game.Static.Entity.Movement.Spline;

namespace NexusForever.Game.Entity.Movement.Spline.Implementation
{
    public interface ISplineMode
    {
        SplineDirection InitialDirection { get; }

        /// <summary>
        /// Get the <see cref="SplineDirection"/> and index of the next point from the specified <see cref="SplineDirection"/> and index.
        /// </summary>
        (SplineDirection Direction, uint Point)? GetNextPoint(SplineTypeBase type, SplineDirection direction, uint point);
    }
}
