using NexusForever.Game.Static.Entity.Movement.Command.State;

namespace NexusForever.Game.Abstract.Entity.Movement.Command.State
{
    public interface IStateCommandGroup : IEntityCommandGroup
    {
        void Initialise(IMovementManager movementManager);

        /// <summary>
        /// Get the current <see cref="StateFlags"/> value.
        /// </summary>
        StateFlags GetState();

        /// <summary>
        /// Set the state flags to the supplied <see cref="StateFlags"/> value.
        /// </summary>
        void SetState(StateFlags state);

        /// <summary>
        /// Set the state to the interpolated <see cref="StateFlags"/> between the supplied times and stage flags.
        /// </summary>
        void SetStateKeys(List<uint> times, List<StateFlags> states);

        /// <summary>
        /// Set the default <see cref="StateFlags"/> value.
        /// </summary>
        void SetStateDefault();
    }
}
