using NexusForever.Game.Abstract.Entity.Movement.AntiTamper;

namespace NexusForever.Game.Entity.Movement.AntiTamper
{
    public class ClientMovementCommandValidator : IClientMovementCommandValidator
    {
        /// <summary>
        /// Validate the time between the client and server to ensure the client is not tampering with the time.
        /// </summary>
        public void ValidateTime(uint clientTime, uint serverTime)
        {
            // TODO
            int difference = (int)clientTime - (int)serverTime;
        }

        /// <summary>
        /// Validate the position from the client to ensure the client is not tampering with the position.
        /// </summary>
        public void ValidatePosition()
        {
            // TODO
        }

        /// <summary>
        /// Validate the mode from the client to ensure the client is not tampering with the mode.
        /// </summary>
        public void ValidateMode()
        {
            // TODO
        }

        /// <summary>
        /// Validate the state from the client to ensure the client is not tampering with the state.
        /// </summary>
        public void ValidateState()
        {
            // TODO
        }
    }
}
