using System.Numerics;
using NexusForever.Game.Abstract.Entity.Movement.Spline.Template;

namespace NexusForever.Game.Entity.Movement.Spline.Template
{
    public class SplineTemplatePoint : ISplineTemplatePoint
    {
        public required Vector3 Position { get; init; }
        public Quaternion? Rotation { get; init; }
        public float? Delay { get; init; }
        public float? FrameTime { get; init; }
    }
}
