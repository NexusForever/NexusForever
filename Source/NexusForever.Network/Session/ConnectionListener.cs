using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using NexusForever.Shared;

namespace NexusForever.Network.Session
{
    public class ConnectionListener<T> : IConnectionListener<T> where T : class, INetworkSession
    {
        /// <summary>
        /// Raised on <see cref="INetworkSession"/> creation for a new client.
        /// </summary>
        public event NewSessionEvent<T> OnNewSession;

        private IPAddress host;
        private uint port;

        private Task listenerTask;
        private readonly ManualResetEventSlim waitHandle = new();
        private volatile CancellationTokenSource cancellationToken;

        #region Dependency Injection

        private readonly ILogger<ConnectionListener<T>> log;
        private readonly IFactory<T> sessionFactory;

        public ConnectionListener(
            ILogger<ConnectionListener<T>> log,
            IFactory<T> sessionFactory)
        {
            this.log            = log;
            this.sessionFactory = sessionFactory;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="ConnectionListener{T}"/> with supplied listening <see cref="IPAddress"/> and port.
        /// </summary>
        public void Initialise(IPAddress host, uint port)
        {
            this.host = host;
            this.port = port;
        }

        /// <summary>
        /// Start listening for new TCP connections.
        /// </summary>
        public void Start()
        {
            if (cancellationToken != null)
                throw new InvalidOperationException();

            cancellationToken = new CancellationTokenSource();

            listenerTask = Task.Factory.StartNew(ListenerThread, TaskCreationOptions.LongRunning);

            // wait for listener task to start before continuing
            waitHandle.Wait();
        }

        private async Task ListenerThread()
        {
            var listener = new TcpListener(host, (int)port);
            listener.Start();

            log.LogInformation($"Started listening for connections on {host}:{port}");

            waitHandle.Set();

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    Socket socket = await listener.AcceptSocketAsync(cancellationToken.Token);

                    T session = sessionFactory.Resolve();
                    session.OnAccept(socket);

                    OnNewSession?.Invoke(session);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception exception)
                {
                    log.LogError(exception, "Error accepting new connection!");
                }
            }

            listener.Stop();

            log.LogInformation($"Stopped listening for connections on {host}:{port}");
        }

        /// <summary>
        /// Shutdown to stop listening for new TCP connections.
        /// </summary>
        public void Shutdown()
        {
            if (cancellationToken == null)
                throw new InvalidOperationException();

            cancellationToken.Cancel();

            listenerTask.Wait();
            listenerTask = null;

            cancellationToken = null;
        }
    }
}
