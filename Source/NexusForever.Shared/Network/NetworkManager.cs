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

        private readonly HashSet<T> sessions = new();

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
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public void Update(double lastTick)
        {
            while (pendingAdd.TryDequeue(out T session))
                sessions.Add(session);

            foreach (T session in sessions)
            {
                session.Update(lastTick);
                if (session.CanDispose())
                    pendingRemove.Enqueue(session);
            }

            while (pendingRemove.TryDequeue(out T session))
                sessions.Remove(session);
        }

        /// <summary>
        /// Return session that meets supplied predicate.
        /// </summary>
        /// <remarks>
        /// An exception will be thrown if multiple sessions meet the predicate.
        /// </remarks>
        public T GetSession(Func<T, bool> func)
        {
            return sessions.SingleOrDefault(func);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return sessions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
