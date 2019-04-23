using NexusForever.WorldServer.Game.Entity.Movement.Spline.Static;

namespace NexusForever.WorldServer.Game.Entity.Movement.Spline.Implementation
{
    [SplineMode(SplineMode.BackAndForth)]
    public class SplineModeBackAndForth : ISplineMode
    {
        public virtual SplineDirection InitialDirection => SplineDirection.Forward;

        public (SplineDirection Direction, uint Point)? GetNextPoint(SplineTypeBase type, SplineDirection direction, uint point)
        {
            if (direction == SplineDirection.Forward && point == type.TopIndex)
                return (SplineDirection.Backward, type.BottomReverseIndex);
            if (direction == SplineDirection.Backward && point == type.TopReverseIndex)
                return (SplineDirection.Forward, type.BottomIndex);

            return (direction, direction == SplineDirection.Forward ? point + 1 : point - 1);
        }
    }
}
