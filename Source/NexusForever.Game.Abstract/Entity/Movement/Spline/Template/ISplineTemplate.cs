using NexusForever.Game.Static.Entity.Movement.Spline;

namespace NexusForever.Game.Abstract.Entity.Movement.Spline.Template
{
    public interface ISplineTemplate
    {
        SplineType Type { get; }
        List<ISplineTemplatePoint> Points { get; }
    }
}