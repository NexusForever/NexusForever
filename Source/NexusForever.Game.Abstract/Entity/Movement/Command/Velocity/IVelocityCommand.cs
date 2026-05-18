using System.Numerics;

namespace NexusForever.Game.Abstract.Entity.Movement.Command.Velocity
{
    public interface IVelocityCommand : IEntityCommand
    {
        /// <summary>
        /// Return the current <see cref="Vector3"/> velocity value for the entity command.
        /// </summary>
        Vector3 GetVelocity();
    }
}
