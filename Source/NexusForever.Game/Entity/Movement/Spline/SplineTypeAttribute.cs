using NexusForever.Game.Static.Entity.Movement.Spline;

namespace NexusForever.Game.Entity.Movement.Spline
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SplineTypeAttribute : Attribute
    {
        public SplineType Type { get; }

        public SplineTypeAttribute(SplineType type)
        {
            Type = type;
        }
    }
}
