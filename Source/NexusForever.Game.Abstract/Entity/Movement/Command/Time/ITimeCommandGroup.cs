namespace NexusForever.Game.Abstract.Entity.Movement.Command.Time
{
    public interface ITimeCommandGroup : IEntityCommandGroup
    {
        /// <summary>
        /// Determines if server time has been reset.
        /// </summary>
        bool TimeReset { get; set; }

        /// <summary>
        /// Initialise <see cref="ITimeCommandGroup"/> with default command.
        /// </summary>
        void Initialise();

        /// <summary>
        /// Get the current server time.
        /// </summary>
        uint GetTime();

        /// <summary>
        /// Reset the current server time to 0.
        /// </summary>
        /// <remarks>
        /// This will reset the time synchronisation information at the client.
        /// </remarks>
        void ResetTime();
    }
}
