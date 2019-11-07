using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NexusForever.Shared.Database.Auth;

namespace NexusForever.Shared.Game
{
    public sealed class ServerManager : Singleton<ServerManager>
    {
        public ImmutableList<ServerInfo> Servers { get; private set; }
        public ImmutableList<ServerMessageInfo> ServerMessages { get; private set; }

        private readonly UpdateTimer checkTimer = new UpdateTimer(60d);
        private readonly UpdateTimer pingCheckTimer = new UpdateTimer(15d);

        private volatile bool shutdownRequested;

        private ServerManager()
        {
        }

        public void Initialise()
        {
            InitialiseServers();
            InitialiseServerMessages();

            var serverManagerThread = new Thread(() =>
            {
                var stopwatch = new Stopwatch();
                double lastTick = 0d;

                while (!shutdownRequested)
                {
                    stopwatch.Restart();

                    Update(lastTick);

                    Thread.Sleep(1000);
                    lastTick = (double)stopwatch.ElapsedTicks / Stopwatch.Frequency;
                }
            });

            serverManagerThread.Start();
        }

        private void InitialiseServers()
        {
            Servers = AuthDatabase.GetServers()
                .Select(s => new ServerInfo(s))
                .ToImmutableList();
        }

        private void InitialiseServerMessages()
        {
            ServerMessages = AuthDatabase.GetServerMessages()
                .GroupBy(m => m.Index)
                .Select(g => new ServerMessageInfo(g))
                .ToImmutableList();
        }

        public void Update(double lastTick)
        {
            checkTimer.Update(lastTick);
            if (checkTimer.HasElapsed)
            {
                InitialiseServers();
                InitialiseServerMessages();

                checkTimer.Reset();
            }

            pingCheckTimer.Update(lastTick);
            if (pingCheckTimer.HasElapsed)
            {
                var tasks = new List<Task>();
                foreach (ServerInfo server in Servers)
                    tasks.Add(server.PingHost(server.Model.Host, server.Model.Port));

                Task.WaitAll(tasks.ToArray());

                pingCheckTimer.Reset();
            }
        }

        public void Shutdown()
        {
            shutdownRequested = true;
        }
    }
}
