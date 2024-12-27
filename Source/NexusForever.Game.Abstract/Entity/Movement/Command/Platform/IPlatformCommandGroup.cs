namespace NexusForever.Game.Abstract.Entity.Movement.Command.Platform
{
    public interface IPlatformCommandGroup : IEntityCommandGroup
    {
        /// <summary>
        /// Initialise <see cref="IPlatformCommandGroup"/ with default command.
        /// </summary>
        void Initialise();

        /// <summary>
        /// Get the current platform unit id value.
        /// </summary>
        uint? GetPlatform();

        /// <summary>
        /// Set the platform to the supplied platform unit id value.
        /// </summary>
        void SetPlatform(uint? platformUnitId);
    }
}
