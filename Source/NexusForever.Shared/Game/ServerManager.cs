using NexusForever.Shared.Configuration;
using NexusForever.Shared.Database;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NexusForever.Shared.Game
{
    public sealed class ServerManager : AbstractManager<ServerManager>
    {
        public ImmutableList<ServerInfo> Servers { get; private set; }
        public ImmutableList<ServerMessageInfo> ServerMessages { get; private set; }

        private ushort? serverRealmId;

        private readonly UpdateTimer checkTimer = new UpdateTimer(60d);
        private readonly UpdateTimer pingCheckTimer = new UpdateTimer(15d);

        private volatile bool shutdownRequested;

        private ServerManager()
        {
        }

        public ServerManager Initialise(ushort? realmId = null)
        {
            serverRealmId = realmId;

            InitialiseServers();
            InitialiseServerMessages();

            // make sure the assigned realm id in the configuration file exists in the database
            if (realmId != null && Instance.Servers.All(s => s.Model.Id != serverRealmId))
                throw new ConfigurationException($"Realm id {realmId} in configuration file doesn't exist in the database!");

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
            return Instance;
        }

        private void InitialiseServers()
        {
            
            Servers = DatabaseManager.Instance.AuthDatabase.GetServers()
                .Select(s => new ServerInfo(s))
                .ToImmutableList();
        }

        private void InitialiseServerMessages()
        {
            ServerMessages = DatabaseManager.Instance.AuthDatabase.GetServerMessages()
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
                Task.WaitAll(Servers
                    .Where(server => server.Model.Id != serverRealmId)
                    .Select(server => server.PingHostAsync())
                    .ToArray());
                pingCheckTimer.Reset();
            }
        }

        /// <summary>
        /// Requests for the <see cref="ServerManager"/> to begin shutdown.
        /// </summary>
        public override void Shutdown()
        {
            shutdownRequested = true;
        }
    }
}
