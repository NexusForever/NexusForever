using System.Numerics;

namespace NexusForever.Game.Abstract.Entity.Movement.Spline.Template
{
    public interface ISplineTemplatePoint
    {
        Vector3 Position { get; init; }
        Quaternion? Rotation { get; init; }
        float? Delay { get; init; }
        float? FrameTime { get; init; }
    }
}
