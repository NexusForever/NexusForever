using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace NexusForever.Shared.Network
{
    public class ConnectionListener<T> where T : NetworkSession, new()
    {
        public delegate void NewSessionEvent(T socket);

        /// <summary>
        /// Raised on <see cref="NetworkSession"/> creation for a new client.
        /// </summary>
        public event NewSessionEvent OnNewSession;

        private volatile bool shutdownRequested;

        public ConnectionListener(IPAddress host, uint port)
        {
            var listener = new TcpListener(host, (int)port);
            listener.Start();

            var thread = new Thread(() =>
            {
                while (!shutdownRequested)
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
            });

            thread.Start();
        }

        public void Shutdown()
        {
            shutdownRequested = true;
        }
    }
}
