using NexusForever.Game.Abstract.Entity.Movement.Spline;
using NexusForever.Game.Abstract.Entity.Movement.Spline.Type;
using NexusForever.Game.Static.Entity.Movement.Spline;

namespace NexusForever.Game.Entity.Movement.Spline.Mode
{
    [SplineMode(SplineMode.OneShot)]
    public class SplineModeOneShot : ISplineMode
    {
        public SplineDirection InitialDirection => SplineDirection.Forward;

        public (SplineDirection Direction, uint Point)? GetNextPoint(ISplineType type, SplineDirection direction, uint point)
        {
            if (point == type.TopIndex)
                return null;

            return (direction, point + 1u);
        }
    }
}
