using System;
using NexusForever.WorldServer.Game.Entity.Movement.Spline.Static;

namespace NexusForever.WorldServer.Game.Entity.Movement.Spline
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
