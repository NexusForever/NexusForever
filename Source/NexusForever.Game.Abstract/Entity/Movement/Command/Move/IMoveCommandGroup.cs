using System.Numerics;

namespace NexusForever.Game.Abstract.Entity.Movement.Command.Move
{
    public interface IMoveCommandGroup : IEntityCommandGroup
    {
        /// <summary>
        /// Initialise <see cref="IMoveCommandGroup"/ with default command.
        /// </summary>
        void Initialise(IMovementManager movementManager);

        /// <summary>
        /// Get the current <see cref="Vector3"/> move value.
        /// </summary>
        Vector3 GetMove();

        /// <summary>
        /// Set the mode to the supplied <see cref="Vector3"/> move value.
        /// </summary>
        void SetMove(Vector3 move, bool blend);

        /// <summary>
        /// Set mode to the interpolated <see cref="Vector3"/> between the supplied times and modes.
        /// </summary>
        void SetMoveKeys(List<uint> times, List<Vector3> moves);

        /// <summary>
        /// Set mode to the default <see cref="Vector3"/> move value.
        /// </summary>
        void SetMoveDefaults(bool blend);
    }
}
