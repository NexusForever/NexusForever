using NexusForever.Game.Static.Entity.Movement.Spline;

namespace NexusForever.Game.Abstract.Entity.Movement.Spline.Type
{
    public interface ISplineTypeFactory
    {
        ISplineType Create(SplineType type);
    }
}
