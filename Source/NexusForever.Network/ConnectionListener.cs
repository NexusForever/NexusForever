using System.Net;
using System.Net.Sockets;
using NLog;

namespace NexusForever.Network
{
    public class ConnectionListener<T> where T : NetworkSession, new()
    {
        private static readonly ILogger log = LogManager.GetLogger($"ConnectionListener<{typeof(T).FullName}>");

        public delegate void NewSessionEvent(T socket);

        /// <summary>
        /// Raised on <see cref="NetworkSession"/> creation for a new client.
        /// </summary>
        public event NewSessionEvent OnNewSession;

        private readonly IPAddress host;
        private readonly uint port;

        private Thread listenerThread;
        private readonly ManualResetEventSlim waitHandle = new();

        private volatile CancellationTokenSource cancellationToken;

        /// <summary>
        /// Create new <see cref="ConnectionListener{T}"/> with supplied listening <see cref="IPAddress"/> and port.
        /// </summary>
        public ConnectionListener(IPAddress host, uint port)
        {
            this.host = host;
            this.port = port;

            cancellationToken = new CancellationTokenSource();

            listenerThread = new Thread(ListenerThread);
            listenerThread.Start();

            // wait for listener thread to start before continuing
            waitHandle.Wait();
        }

        private void ListenerThread()
        {
            var listener = new TcpListener(host, (int)port);
            listener.Start();

            log.Info($"Started listening for connections on {host}:{port}");

            waitHandle.Set();

            while (!cancellationToken.IsCancellationRequested)
            {
                while (listener.Pending())
                {
                    var session = new T();
                    session.OnAccept(listener.AcceptSocket());

                    OnNewSession?.Invoke(session);
                }

                Thread.Sleep(1);
            }

            listener.Stop();

            log.Info($"Stopped listening for connections on {host}:{port}");
        }

        /// <summary>
        /// Shutdown to stop listening for new TCP connections.
        /// </summary>
        public void Shutdown()
        {
            if (cancellationToken == null)
                throw new InvalidOperationException();

            cancellationToken.Cancel();

            listenerThread.Join();
            listenerThread = null;

            cancellationToken = null;
        }
    }
}
