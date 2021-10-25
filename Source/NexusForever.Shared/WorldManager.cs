using System;
using System.Diagnostics;
using System.Threading;
using NLog;

namespace NexusForever.Shared
{
    public sealed class WorldManager : Singleton<WorldManager>
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private Thread worldThread;
        private readonly ManualResetEventSlim waitHandle = new();

        private volatile CancellationTokenSource cancellationToken;

        private WorldManager()
        {
        }

        /// <summary>
        /// Initialise <see cref="WorldManager"/> and any related resources.
        /// </summary>
        public void Initialise(Action<double> updateAction)
        {
            if (cancellationToken != null)
                throw new InvalidOperationException();

            log.Info("Initialising world manager...");

            cancellationToken = new CancellationTokenSource();

            worldThread = new Thread(() => WorldThread(updateAction));
            worldThread.Start();

            // wait for world thread to start before continuing
            waitHandle.Wait();
        }

        private void WorldThread(Action<double> updateAction)
        {
            log.Info("Started world thread.");
            waitHandle.Set();

            var stopwatch = new Stopwatch();
            double lastTick = 0d;

            while (!cancellationToken.IsCancellationRequested)
            {
                stopwatch.Restart();

                updateAction(lastTick);

                Thread.Sleep(1);
                lastTick = (double)stopwatch.ElapsedTicks / Stopwatch.Frequency;
            }

            log.Info("Stopped world thread.");
        }

        /// <summary>
        /// Request shutdown of <see cref="WorldManager"/> and any related resources.
        /// </summary>
        public void Shutdown()
        {
            if (cancellationToken == null)
                throw new InvalidOperationException();

            log.Info("Shutting down world manager...");

            cancellationToken.Cancel();

            worldThread.Join();
            worldThread = null;

            cancellationToken = null;
        }
    }
}
