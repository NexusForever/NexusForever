using System.Numerics;

namespace NexusForever.Game.Abstract.Entity.Movement.Command.Velocity
{
    public interface IVelocityCommandGroup : IEntityCommandGroup
    {
        /// <summary>
        /// Initialise <see cref="IVelocityCommandGroup"/ with default command.
        /// </summary>
        void Initialise();

        /// <summary>
        /// Get the current <see cref="Vector3"/> velocity value.
        /// </summary>
        Vector3 GetVelocity();

        /// <summary>
        /// Set the velocity to the supplied <see cref="Vector3"/> value.
        /// </summary>
        void SetVelocity(Vector3 velocity, bool blend);

        /// <summary>
        /// Set velocity to the interpolated <see cref="Vector3"/> velocity between the supplied times and velocities.
        /// </summary>
        void SetVelocityKeys(List<uint> times, List<Vector3> modes);

        /// <summary>
        /// Set velocity to the default values.
        /// </summary>
        void SetVelocityDefaults();
    }
}
