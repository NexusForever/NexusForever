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

            var listener = new TcpListener(host, (int)port);
            var source = new CancellationTokenSource();
            try
            {
                // Bind synchronously so startup failures reach the caller.
                listener.Start();
                log.LogInformation($"Started listening for connections on {host}:{port}");

                CancellationToken token = source.Token;
                cancellationToken = source;
                listenerTask = Task.Run(() => ListenerThread(listener, token));
            }
            catch
            {
                listener.Stop();
                source.Dispose();
                cancellationToken = null;
                throw;
            }
        }

        private async Task ListenerThread(TcpListener listener, CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        Socket socket = await listener.AcceptSocketAsync(token);

                        T session = sessionFactory.Resolve();
                        session.OnAccept(socket);

                        OnNewSession?.Invoke(session);
                    }
                    catch (OperationCanceledException) when (token.IsCancellationRequested)
                    {
                        break;
                    }
                    catch (Exception exception)
                    {
                        log.LogError(exception, "Error accepting new connection!");
                    }
                }
            }
            finally
            {
                listener.Stop();
                log.LogInformation($"Stopped listening for connections on {host}:{port}");
            }
        }

        /// <summary>
        /// Shutdown to stop listening for new TCP connections.
        /// </summary>
        public void Shutdown()
        {
            if (cancellationToken == null)
                throw new InvalidOperationException();

            cancellationToken.Cancel();

            try
            {
                listenerTask.Wait();
            }
            finally
            {
                cancellationToken.Dispose();
                listenerTask = null;
                cancellationToken = null;
            }
        }
    }
}
