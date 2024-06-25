using System.Net;

namespace NexusForever.Network.Session
{
    public interface IConnectionListener<T> where T : class, INetworkSession
    {
        event NewSessionEvent<T> OnNewSession;

        /// <summary>
        /// Initialise <see cref="IConnectionListener{T}"/> with supplied listening <see cref="IPAddress"/> and port.
        /// </summary>
        void Initialise(IPAddress host, uint port);

        /// <summary>
        /// Start listening for new TCP connections.
        /// </summary>
        void Start();

        /// <summary>
        /// Shutdown to stop listening for new TCP connections.
        /// </summary>
        void Shutdown();
    }
}
