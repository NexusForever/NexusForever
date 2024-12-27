using NexusForever.Game.Abstract.Entity.Movement.Spline.Template;

namespace NexusForever.Game.Abstract.Entity.Movement.Spline
{
    public interface ISplinePoint
    {
        int Index { get; init; }
        ISplineTemplatePoint TemplatePoint { get; init; }
        List<ISplinePointLength> Lengths { get; }
        List<float> Offsets { get; }
        float FrameTime { get; set; }
    }
}