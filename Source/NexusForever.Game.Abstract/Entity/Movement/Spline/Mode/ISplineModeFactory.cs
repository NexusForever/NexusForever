using NexusForever.Game.Static.Entity.Movement.Spline;

namespace NexusForever.Game.Abstract.Entity.Movement.Spline.Mode
{
    public interface ISplineModeFactory
    {
        ISplineMode Create(SplineMode mode);
    }
}
