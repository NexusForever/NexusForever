using System.Collections.Immutable;
using System.Diagnostics;
using NexusForever.Database;
using NexusForever.Database.Auth;
using NexusForever.Game.Abstract.Server;
using NexusForever.Shared;
using NexusForever.Shared.Configuration;
using NexusForever.Shared.Game;
using NLog;

namespace NexusForever.Game.Server
{
    public sealed class ServerManager : Singleton<ServerManager>, IServerManager
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public ImmutableList<IServerInfo> Servers { get; private set; }
        public ImmutableList<IServerMessageInfo> ServerMessages { get; private set; }

        private ushort? serverRealmId;

        private readonly UpdateTimer checkTimer = new(60d);
        private readonly UpdateTimer pingCheckTimer = new(15d);

        private Thread serverPollThread;
        private readonly ManualResetEventSlim waitHandle = new();

        private volatile CancellationTokenSource cancellationToken;

        private ServerManager()
        {
        }

        /// <summary>
        /// Initialise <see cref="IServerManager"/> and any related resources.
        /// </summary>
        public void Initialise(ushort? realmId = null)
        {
            if (cancellationToken != null)
                throw new InvalidOperationException();

            log.Info("Initialising server manager...");

            serverRealmId = realmId;

            InitialiseServers();
            InitialiseServerMessages();

            // make sure the assigned realm id in the configuration file exists in the database
            if (realmId != null && Servers.All(s => s.Model.Id != realmId))
                throw new ConfigurationException($"Realm id {realmId} in configuration file doesn't exist in the database!");

            cancellationToken = new CancellationTokenSource();

            serverPollThread = new Thread(ServerPollThread);
            serverPollThread.Start();

            // wait for poll thread to start before continuing
            waitHandle.Wait();
        }

        private void InitialiseServers()
        {
            Servers = DatabaseManager.Instance.GetDatabase<AuthDatabase>().GetServers()
                .Select(s => (IServerInfo)new ServerInfo(s))
                .ToImmutableList();
        }

        private void InitialiseServerMessages()
        {
            ServerMessages = DatabaseManager.Instance.GetDatabase<AuthDatabase>().GetServerMessages()
                .GroupBy(m => m.Index)
                .Select(g => (IServerMessageInfo)new ServerMessageInfo(g))
                .ToImmutableList();
        }

        private void ServerPollThread()
        {
            waitHandle.Set();

            var stopwatch = new Stopwatch();
            double lastTick = 0d;

            while (!cancellationToken.IsCancellationRequested)
            {
                stopwatch.Restart();

                // check for updates to the server definitions in the database every 60 seconds
                checkTimer.Update(lastTick);
                if (checkTimer.HasElapsed)
                {
                    InitialiseServers();
                    InitialiseServerMessages();

                    checkTimer.Reset();
                }

                // check for server heartbeats every 15 seconds
                pingCheckTimer.Update(lastTick);
                if (pingCheckTimer.HasElapsed)
                {
                    Task.WaitAll(Servers
                        .Where(server => server.Model.Id != serverRealmId)
                        .Select(server => server.PingHostAsync())
                        .ToArray());
                    pingCheckTimer.Reset();
                }

                Thread.Sleep(1000);
                lastTick = (double)stopwatch.ElapsedTicks / Stopwatch.Frequency;
            }
        }

        /// <summary>
        /// Requests for the <see cref="IServerManager"/> to begin shutdown.
        /// </summary>
        public void Shutdown()
        {
            if (cancellationToken == null)
                throw new InvalidOperationException();

            log.Info("Shutting down server manager...");

            cancellationToken.Cancel();

            serverPollThread.Join();
            serverPollThread = null;

            cancellationToken = null;
        }
    }
}
