using System.Numerics;

namespace NexusForever.Game.Abstract.Entity.Movement.Spline
{
    public interface ISplinePoint
    {
        Vector3 Position { get; }
        float Length { get; set; }
    }
}