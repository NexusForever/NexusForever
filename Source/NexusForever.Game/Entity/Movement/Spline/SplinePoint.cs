using NexusForever.Game.Abstract.Entity.Movement.Spline;
using NexusForever.Game.Abstract.Entity.Movement.Spline.Template;

namespace NexusForever.Game.Entity.Movement.Spline
{
    public class SplinePoint : ISplinePoint
    {
        public required int Index { get; init; }
        public required ISplineTemplatePoint TemplatePoint { get; init; }
        public List<ISplinePointLength> Lengths { get; } = new();
        public List<float> Offsets { get; } = new();
        public float FrameTime { get; set; }
    }
}
