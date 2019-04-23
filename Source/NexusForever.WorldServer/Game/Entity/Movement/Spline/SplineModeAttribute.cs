using System;
using NexusForever.WorldServer.Game.Entity.Movement.Spline.Static;

namespace NexusForever.WorldServer.Game.Entity.Movement.Spline
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
