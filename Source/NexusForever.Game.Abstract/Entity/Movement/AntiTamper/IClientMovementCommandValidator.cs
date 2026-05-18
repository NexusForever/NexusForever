namespace NexusForever.Game.Abstract.Entity.Movement.AntiTamper
{
    public interface IClientMovementCommandValidator
    {
        /// <summary>
        /// Validate the time between the client and server to ensure the client is not tampering with the time.
        /// </summary>
        void ValidateTime(uint clientTime, uint serverTime);

        /// <summary>
        /// Validate the position from the client to ensure the client is not tampering with the position.
        /// </summary>
        void ValidatePosition();

        /// <summary>
        /// Validate the mode from the client to ensure the client is not tampering with the mode.
        /// </summary>
        void ValidateMode();

        /// <summary>
        /// Validate the state from the client to ensure the client is not tampering with the state.
        /// </summary>
        void ValidateState();
    }
}
