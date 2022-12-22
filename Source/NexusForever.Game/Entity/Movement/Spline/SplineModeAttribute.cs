using NexusForever.Game.Static.Entity.Movement.Spline;

namespace NexusForever.Game.Entity.Movement.Spline
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SplineModeAttribute : Attribute
    {
        public SplineMode Mode { get; }

        public SplineModeAttribute(SplineMode mode)
        {
            Mode = mode;
        }
    }
}
