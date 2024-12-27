using System.Numerics;
using NexusForever.Game.Abstract.Entity.Movement.Command.Position;

namespace NexusForever.Game.Abstract.Entity.Movement.Command.Rotation
{
    public interface IRotationCommandGroup : IEntityCommandGroup
    {
        /// <summary>
        /// Initialise <see cref="IRotationCommandGroup"/>.
        /// </summary>
        void Initialise(IMovementManager movementManager, IPositionCommandGroup positionCommandGroup);

        /// <summary>
        /// Get the current <see cref="Vector3"/> rotation value.
        /// </summary>
        Vector3 GetRotation();

        /// <summary>
        /// Set rotation to the supplied <see cref="Vector3"/> value.
        /// </summary>
        void SetRotation(Vector3 rotation, bool blend);

        /// <summary>
        /// Set rotation to the interpolated value between the supplied times and rotations.
        /// </summary>
        void SetRotationKeys(List<uint> times, List<Vector3> rotations);

        /// <summary>
        /// NYI
        /// </summary>
        void SetRotationSpline();

        /// <summary>
        /// NYI
        /// </summary>
        void SetRotationMultiSpline();

        /// <summary>
        /// Set rotation value to face the supplied entity.
        /// </summary>
        void SetRotationFaceUnit(uint faceUnitId);

        /// <summary>
        /// Set rotation value to face the supplied position.
        /// </summary>
        void SetRotationFacePosition(Vector3 position);

        /// <summary>
        /// Set the rotation value to the supplied spin.
        /// </summary>
        /// <remarks>
        /// <paramref name="rotation"/> is the initial rotation value.
        /// <paramref name="spin"/> is the amount of radians to spin per second.
        /// </remarks>
        void SetRotationSpin(Vector3 rotation, TimeSpan duration, float spin);

        /// <summary>
        /// Set rotation with the default values.
        /// </summary>
        /// <remarks>
        /// Rotation will be the direction of movement of paths, splines or keys.
        /// </remarks>
        void SetRotationDefaults();
    }
}
