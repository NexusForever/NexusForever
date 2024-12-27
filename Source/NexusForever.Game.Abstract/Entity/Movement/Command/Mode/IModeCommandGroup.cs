using NexusForever.Game.Static.Entity.Movement.Command.Mode;

namespace NexusForever.Game.Abstract.Entity.Movement.Command.Mode
{
    public interface IModeCommandGroup : IEntityCommandGroup
    {
        /// <summary>
        /// Initialise <see cref="IModeCommandGroup"/ with default command.
        /// </summary>
        void Initialise(IMovementManager movementManager);

        /// <summary>
        /// Get the current <see cref="ModeType"/> value.
        /// </summary>
        ModeType GetMode();

        /// <summary>
        /// Set the mode to the supplied <see cref="ModeType"/> value.
        /// </summary>
        void SetMode(ModeType mode);

        /// <summary>
        /// Set <see cref="ModeType"/> to the interpolated value between the supplied times and modes.
        /// </summary>
        void SetModeKeys(List<uint> times, List<ModeType> modes);

        /// <summary>
        /// Set mode to the default <see cref="ModeType"/> value.
        /// </summary>
        void SetModeDefault();
    }
}
