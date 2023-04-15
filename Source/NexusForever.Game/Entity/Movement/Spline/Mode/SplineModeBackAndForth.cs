using NexusForever.Game.Abstract.Entity.Movement.Spline;
using NexusForever.Game.Abstract.Entity.Movement.Spline.Type;
using NexusForever.Game.Static.Entity.Movement.Spline;

namespace NexusForever.Game.Entity.Movement.Spline.Mode
{
    [SplineMode(SplineMode.BackAndForth)]
    public class SplineModeBackAndForth : ISplineMode
    {
        public virtual SplineDirection InitialDirection => SplineDirection.Forward;

        public (SplineDirection Direction, uint Point)? GetNextPoint(ISplineType type, SplineDirection direction, uint point)
        {
            if (direction == SplineDirection.Forward && point == type.TopIndex)
                return (SplineDirection.Backward, type.BottomReverseIndex);
            if (direction == SplineDirection.Backward && point == type.TopReverseIndex)
                return (SplineDirection.Forward, type.BottomIndex);

            return (direction, direction == SplineDirection.Forward ? point + 1 : point - 1);
        }
    }
}
