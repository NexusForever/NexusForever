using System;
using System.Diagnostics;
using System.Threading;

namespace NexusForever.Shared
{
    public sealed class WorldManager : Singleton<WorldManager>
    {
        private volatile bool shutdownRequested;

        private WorldManager()
        {
        }

        public void Initialise(Action<double> updateAction)
        {
            var worldThread = new Thread(() =>
            {
                var stopwatch = new Stopwatch();
                double lastTick = 0d;

                while (!shutdownRequested)
                {
                    stopwatch.Restart();

                    updateAction(lastTick);

                    Thread.Sleep(1);
                    lastTick = (double)stopwatch.ElapsedTicks / Stopwatch.Frequency;
                }
            });

            worldThread.Start();
        }

        public void Shutdown()
        {
            shutdownRequested = true;
        }
    }
}
