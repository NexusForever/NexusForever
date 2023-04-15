using System.Numerics;
using NexusForever.Game.Static.Entity.Movement.Spline;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Entity.Movement.Spline
{
    public interface ISplinePath : IUpdate
    {
        SplineType Type { get; }
        SplineMode Mode { get; }
        float Speed { get; }
        float Length { get; }
        float Position { get; }
        bool IsFinialised { get; }

        /// <summary>
        /// Get current <see cref="Vector3"/> position on spline.
        /// </summary>
        Vector3 GetPosition();
    }
}