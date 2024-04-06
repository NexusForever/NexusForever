using System.Numerics;

namespace NexusForever.Game.Abstract.Entity.Movement.Command.Position
{
    public interface IPositionCommand : IEntityCommand
    {
        /// <summary>
        /// Returns the current <see cref="Vector3"/> position for the entity command.
        /// </summary>
        Vector3 GetPosition();

        /// <summary>
        /// Returns the current <see cref="Vector3"/> rotation for the entity command.
        /// </summary>
        /// <remarks>
        /// This is used to calculate the direction of travel for movement commands.
        /// </remarks>
        Vector3 GetRotation();
    }
}
