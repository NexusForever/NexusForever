using NexusForever.Game.Static.Entity.Movement.Spline;

namespace NexusForever.Game.Abstract.Entity.Movement.Spline.Mode
{
    public interface ISplineModeInterpolatedOffset
    {
        float Offset { get; set; }
        SplineDirection Direction { get; set; }
        bool Finalised { get; set; }
    }
}