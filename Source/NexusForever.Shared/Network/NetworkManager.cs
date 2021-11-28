using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using NexusForever.Shared.Configuration;
using NLog;

namespace NexusForever.Shared.Network
{
    public sealed class NetworkManager<T> : Singleton<NetworkManager<T>>, IEnumerable<T>, IUpdate where T : NetworkSession, new()
    {
        private static readonly ILogger log = LogManager.GetLogger($"NetworkManager<{typeof(T).FullName}>");

        private ConnectionListener<T> connectionListener;

        private readonly ConcurrentQueue<T> pendingAdd = new();
        private readonly Queue<T> pendingRemove = new();
        private readonly Queue<(T, string)> pendingUpdate = new();

        private readonly Dictionary<string, T> sessions = new();

        private NetworkManager()
        {
        }

        /// <summary>
        /// Initialise <see cref="NetworkManager{T}"/> and any related resources.
        /// </summary>
        public void Initialise(NetworkConfig config)
        {
            if (connectionListener != null)
                throw new InvalidOperationException();

            log.Info("Initialising network manager...");

            connectionListener = new ConnectionListener<T>(IPAddress.Parse(config.Host), config.Port);
            connectionListener.OnNewSession += session => pendingAdd.Enqueue(session);
        }

        /// <summary>
        /// Shutdown <see cref="NetworkManager{T}"/> and any related resources.
        /// </summary>
        public void Shutdown()
        {
            if (connectionListener == null)
                throw new InvalidOperationException();

            log.Info("Shutting down network manager...");

            connectionListener.Shutdown();
            connectionListener = null;
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
                sessions.Add(session.Id, session);

            foreach (T session in sessions.Values)
            {
                session.Update(lastTick);
                if (session.CanDispose())
                    pendingRemove.Enqueue(session);
            }

            while (pendingUpdate.TryDequeue(out (T Session, string Id) update))
            {
                sessions.Remove(update.Session.Id);
                update.Session.UpdateId(update.Id);
                sessions.Add(update.Id, update.Session);
            }

            while (pendingRemove.TryDequeue(out T session))
                sessions.Remove(session.Id);
        }

        /// <summary>
        /// Return session with supplied id.
        /// </summary>
        public T GetSession(string id)
        {
            return sessions.TryGetValue(id, out T session) ? session : null;
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
