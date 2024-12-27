using System.Numerics;
using NexusForever.Game.Abstract.Entity.Movement.Spline.Mode;
using NexusForever.Game.Abstract.Entity.Movement.Spline.Template;
using NexusForever.Game.Abstract.Entity.Movement.Spline.Type;
using NexusForever.Game.Static.Entity.Movement.Spline;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Entity.Movement.Spline
{
    public interface ISpline : IUpdate
    {
        /// <summary>
        /// Returns if the spline has been finalised.
        /// </summary>
        bool IsFinialised { get; }

        List<ISplinePoint> Points { get; }
        ISplineType Type { get; }
        ISplineMode Mode { get; }
        float Speed { get; }
        SplineDirection Direction { get; }

        /// <summary>
        /// Position on the spline.
        /// </summary>
        /// <remarks>
        /// Value will be between 0 and the total length of the spline.
        /// </remarks>
        float Position { get; }

        /// <summary>
        /// Offset on the spline.
        /// </summary>
        /// <remarks>
        /// Value will be between 0 and t max (usually 1).
        /// </remarks>
        float Offset { get; }

        /// <summary>
        /// Initialise the spline with the supplied <see cref="ISplineTemplate"/>, <see cref="SplineMode"/> and speed.
        /// </summary>
        void Initialise(ISplineTemplate template, SplineMode mode, float speed);

        /// <summary>
        /// Get current position on the spline.
        /// </summary>
        Vector3 GetPosition();

        /// <summary>
        /// Get current rotation on the spline.
        /// </summary>
        Vector3 GetRotation();
    }
}
