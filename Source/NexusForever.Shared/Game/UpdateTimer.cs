namespace NexusForever.Shared.Game
{
    public class UpdateTimer : IUpdate
    {
        public bool HasElapsed => Time <= 0d;
        public double Time { get; private set; }
        public bool IsTicking { get; private set; }

        private readonly double duration;

        /// <summary>
        /// Create a new <see cref="UpdateTimer"/> with supplied starting duration.
        /// </summary>
        public UpdateTimer(double duration, bool start = true)
        {
            this.duration = duration;
            Time          = duration;
            IsTicking     = start;
        }

        public void Update(double lastTick)
        {
            if (!IsTicking)
                return;

            Time -= lastTick;
            if (Time <= 0d)
            {
                Time = 0d;
                IsTicking = false;
            }
        }

        /// <summary>
        /// Reset timer, setting duration to the default value.
        /// </summary>
        public void Reset(bool start = true)
        {
            Time = duration;
            IsTicking = start;
        }

        /// <summary>
        /// Resume timer allowing it to tick during an update.
        /// </summary>
        public void Resume()
        {
            IsTicking = true;
        }

        /// <summary>
        /// Pause timer preventing it from ticking during an update.
        /// </summary>
        public void Pause()
        {
            IsTicking = false;
        }
    }
}
