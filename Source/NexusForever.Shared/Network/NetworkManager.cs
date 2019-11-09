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

        private readonly ConcurrentQueue<T> pendingAdd = new ConcurrentQueue<T>();
        private readonly ConcurrentQueue<T> pendingRemove = new ConcurrentQueue<T>();

        private readonly HashSet<T> sessions = new HashSet<T>();

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

        public void Update(double lastTick)
        {
            //
            while (pendingAdd.TryDequeue(out T session))
                sessions.Add(session);

            //
            foreach (T session in sessions)
            {
                session.Update(lastTick);
                if (session.Disconnected && !session.PendingEvent)
                    pendingRemove.Enqueue(session);
            }

            //
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
