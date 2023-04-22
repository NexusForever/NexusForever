using NexusForever.Game.Abstract.Entity.Movement.Spline.Type;
using NexusForever.Game.Static.Entity.Movement.Spline;

namespace NexusForever.Game.Abstract.Entity.Movement.Spline
{
    public interface ISplineMode
    {
        SplineDirection InitialDirection { get; }

        /// <summary>
        /// Get the <see cref="SplineDirection"/> and index of the next point from the specified <see cref="SplineDirection"/> and index.
        /// </summary>
        (SplineDirection Direction, uint Point)? GetNextPoint(ISplineType type, SplineDirection direction, uint point);
    }
}
