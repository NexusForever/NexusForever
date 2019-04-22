using NexusForever.WorldServer.Game.Entity.Movement.Spline.Static;

namespace NexusForever.WorldServer.Game.Entity.Movement.Spline.Implementation
{
    [SplineMode(SplineMode.Cyclic)]
    public class SplineModeCyclic : ISplineMode
    {
        public SplineDirection InitialDirection => SplineDirection.Forward;

        public (SplineDirection Direction, uint Point)? GetNextPoint(SplineTypeBase type, SplineDirection direction, uint point)
        {
            if (point == type.TopIndex)
                return (direction, type.BottomIndex);

            return (direction, point + 1u);
        }
    }
}
