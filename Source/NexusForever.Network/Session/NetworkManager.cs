using System.Collections;
using System.Collections.Concurrent;
using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NexusForever.Network.Configuration.Model;

namespace NexusForever.Network.Session
{
    public sealed class NetworkManager<T> : INetworkManager<T> where T : class, INetworkSession
    {
        private readonly ConcurrentQueue<T> pendingAdd = new();
        private readonly Queue<T> pendingRemove = new();
        private readonly Queue<(T, string)> pendingUpdate = new();

        private readonly Dictionary<string, T> sessions = new();

        #region Depedency Injection

        private readonly ILogger<NetworkManager<T>> log;
        private readonly IOptions<NetworkConfig> networkOptions;

        private readonly IConnectionListener<T> connectionListener;

        public NetworkManager(
            ILogger<NetworkManager<T>> log,
            IOptions<NetworkConfig> networkOptions,
            IConnectionListener<T> connectionListener)
        {
            this.log                = log;
            this.networkOptions     = networkOptions;

            this.connectionListener = connectionListener;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="INetworkManager{T}"/> and any related resources.
        /// </summary>
        public void Initialise()
        {
            log.LogInformation("Initialising network manager...");

            connectionListener.Initialise(IPAddress.Parse(networkOptions.Value.Host), networkOptions.Value.Port);
            connectionListener.OnNewSession += pendingAdd.Enqueue;
        }

        /// <summary>
        /// Start <see cref="INetworkManager{T}"/> and any related resources.
        /// </summary>
        public void Start()
        {
            log.LogInformation("Starting network manager...");

            connectionListener.Start();
        }

        /// <summary>
        /// Shutdown <see cref="INetworkManager{T}"/> and any related resources.
        /// </summary>
        public void Shutdown()
        {
            log.LogInformation("Shutting down network manager...");

            connectionListener.Shutdown();
        }

        /// <summary>
        /// Update session existing id with a new supplied id.
        /// </summary>
        /// <remarks>
        /// This should be used when the default session id can be replaced with a known unique id.
        /// </remarks>
        public void UpdateSessionId(T session, string id)
        {
            pendingUpdate.Enqueue((session, id));
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public void Update(double lastTick)
        {
            while (pendingAdd.TryDequeue(out T session))
                AddSession(session);

            foreach (T session in sessions.Values)
            {
                session.Update(lastTick);
                if (session.CanDispose())
                    pendingRemove.Enqueue(session);
            }

            while (pendingUpdate.TryDequeue(out (T Session, string Id) update))
                UpdateSession(update.Session, update.Id);

            while (pendingRemove.TryDequeue(out T session))
            {
                sessions.Remove(session.Id);
                log.LogTrace($"Removed session {session.Id}.");
            }
        }

        private void AddSession(T session)
        {
            if (sessions.TryGetValue(session.Id, out T existingSession))
            {
                // there is already an existing session with this key, disconnect it
                log.LogTrace($"New session with id {session.Id} conflicts with existing session.");

                existingSession.ForceDisconnect();
                UpdateSession(existingSession, Guid.NewGuid().ToString());
            }

            sessions.Add(session.Id, session);
            log.LogTrace($"Added session {session.Id}.");
        }

        private void UpdateSession(T session, string id)
        {
            sessions.Remove(session.Id);
            session.UpdateId(id);
            AddSession(session);
        }

        /// <summary>
        /// Return session with supplied id.
        /// </summary>
        public T GetSession(string id)
        {
            return sessions.TryGetValue(id, out T session) ? session : default;
        }

        /// <summary>
        /// Return session that meets supplied predicate.
        /// </summary>
        /// <remarks>
        /// An exception will be thrown if multiple sessions meet the predicate.
        /// </remarks>
        public T GetSession(Func<T, bool> func)
        {
            return sessions.Values.SingleOrDefault(func);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return sessions.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
