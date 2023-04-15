using NexusForever.Game.Abstract.Entity.Movement.Spline;
using NexusForever.Game.Abstract.Entity.Movement.Spline.Type;
using NexusForever.Game.Static.Entity.Movement.Spline;

namespace NexusForever.Game.Entity.Movement.Spline.Mode
{
    [SplineMode(SplineMode.Cyclic)]
    public class SplineModeCyclic : ISplineMode
    {
        public SplineDirection InitialDirection => SplineDirection.Forward;

        public (SplineDirection Direction, uint Point)? GetNextPoint(ISplineType type, SplineDirection direction, uint point)
        {
            if (point == type.TopIndex)
                return (direction, type.BottomIndex);

            return (direction, point + 1u);
        }
    }
}
