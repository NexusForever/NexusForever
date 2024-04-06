using System.Numerics;

namespace NexusForever.Game.Abstract.Entity.Movement.Command.Rotation
{
    public interface IRotationCommand : IEntityCommand
    {
        /// <summary>
        /// Returns the current <see cref="Vector3"/> rotation for the entity command.
        /// </summary>
        Vector3 GetRotation();
    }
}
