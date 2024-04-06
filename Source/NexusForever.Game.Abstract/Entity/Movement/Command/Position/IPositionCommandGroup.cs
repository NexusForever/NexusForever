using System.Numerics;
using NexusForever.Game.Static.Entity.Movement.Spline;

namespace NexusForever.Game.Abstract.Entity.Movement.Command.Position
{
    public interface IPositionCommandGroup : IEntityCommandGroup
    {
        /// <summary>
        /// Current position entity command.
        /// </summary>
        IPositionCommand Command { get; }

        /// <summary>
        /// Initialise <see cref="IPositionCommandGroup"/ with default command.
        /// </summary>
        void Initialise(IMovementManager movementManager);

        /// <summary>
        /// Get the current <see cref="Vector3"/> position value.
        /// </summary>
        Vector3 GetPosition();

        /// <summary>
        /// Set the position to the supplied <see cref="Vector3"/> value.
        /// </summary>
        void SetPosition(Vector3 position, bool blend);

        /// <summary>
        /// Set the position to the interpolated <see cref="Vector3"/> between the supplied times and positions.
        /// </summary>
        void SetPositionKeys(List<uint> times, List<Vector3> positions);

        /// <summary>
        /// Set the position based on the supplied nodes, <see cref="SplineType"/>, <see cref="SplineMode"/> and speed.
        /// </summary>
        void SetPositionPath(List<Vector3> nodes, SplineType type, SplineMode mode, float speed);

        /// <summary>
        /// Set the position based on the supplied spline, <see cref="SplineMode"/> and speed.
        /// </summary>
        void SetPositionSpline(ushort splineId, SplineMode mode, float speed);

        /// <summary>
        /// NYI
        /// </summary>
        void SetPositionMultiSpline();

        /// <summary>
        /// NYI
        /// </summary>
        void SetPositionProjectile();
    }
}
