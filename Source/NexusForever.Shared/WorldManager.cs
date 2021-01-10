using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace NexusForever.Shared
{
    public sealed class WorldManager : AbstractManager<WorldManager>
    {
        private volatile bool shutdownRequested;
        private List<IShutdown> managersList;

        private WorldManager()
        {
        }

        public void Initialise(Action<double> updateAction, List<IShutdown> shutdownAbles)
        {
            managersList = shutdownAbles;
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

                for (int i = managersList.Count - 1; i >= 0; i--)
                {
                    Log.Trace($"Shutting down {managersList[i]}");
                    managersList[i].Shutdown();
                }

                ShutdownRequest();
            });

            worldThread.Start();
        }

        public override void Shutdown()
        {
            shutdownRequested = true;
        }

        public delegate void OnShutdownHandler();
        public event OnShutdownHandler OnShutdown;
        private void ShutdownRequest()
        {
            if (OnShutdown != null)
            {
                Delegate[] subscribers = OnShutdown.GetInvocationList();
                foreach (var @delegate in subscribers)
                {
                    var subscriber = (OnShutdownHandler) @delegate;
                    try
                    {
                        subscriber();
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
                Log.Info("Shutdown Complete");
                LogManager.Shutdown();
            }
        }
    }
}
