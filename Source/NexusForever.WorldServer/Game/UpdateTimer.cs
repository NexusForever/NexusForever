using NexusForever.Shared;

namespace NexusForever.WorldServer.Game
{
    public class UpdateTimer : IUpdate
    {
        public bool HasElapsed => time <= 0d;

        private readonly double duration;

        private double time;
        private bool isTicking;

        public UpdateTimer(double duration, bool start = true)
        {
            this.duration = duration;
            isTicking = start;
        }

        public void Update(double lastTick)
        {
            if (!isTicking)
                return;

            time -= lastTick;
            if (time <= 0d)
                isTicking = false;
        }

        public void Reset()
        {
            time = duration;
            isTicking = true;
        }

        public void Resume()
        {
            isTicking = true;
        }

        public void Pause()
        {
            isTicking = false;
        }
    }
}
