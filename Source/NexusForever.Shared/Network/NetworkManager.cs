using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using NexusForever.Shared.Configuration;

namespace NexusForever.Shared.Network
{
    public sealed class NetworkManager<T> : Singleton<NetworkManager<T>>, IUpdate where T : NetworkSession, new()
    {
        private ConnectionListener<T> connectionListener;

        private readonly ConcurrentQueue<T> pendingAdd = new();
        private readonly Queue<T> pendingRemove = new();

        private readonly HashSet<T> sessions = new();

        private NetworkManager()
        {
        }

        public void Initialise(NetworkConfig config)
        {
            connectionListener = new ConnectionListener<T>(IPAddress.Parse(config.Host), config.Port);
            connectionListener.OnNewSession += (session) =>
            {
                pendingAdd.Enqueue(session);
            };
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

        public T GetSession(Func<T, bool> func)
        {
            return sessions.SingleOrDefault(func);
        }

        public IEnumerable<T> GetSessions()
        {
            return sessions;
        }

        public IEnumerable<T> GetSessions(Func<T, bool> func)
        {
            return sessions.Where(func);
        }

        public void Shutdown()
        {
            connectionListener?.Shutdown();
        }
    }
}
