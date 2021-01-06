using NexusForever.Shared.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace NexusForever.Shared.Network
{
    public sealed class NetworkManager<T> : AbstractManager<NetworkManager<T>>, IUpdate where T : NetworkSession, new()
    {
        private ConnectionListener<T> connectionListener;

        private readonly ConcurrentQueue<T> pendingAdd = new ConcurrentQueue<T>();
        private readonly ConcurrentQueue<T> pendingRemove = new ConcurrentQueue<T>();
        private readonly HashSet<T> sessions = new HashSet<T>();

        private NetworkManager()
        {
        }

        public NetworkManager<T> Initialise(NetworkConfig config)
        {
            connectionListener = new ConnectionListener<T>(IPAddress.Parse(config.Host), config.Port);
            connectionListener.OnNewSession += (session) =>
            {
                pendingAdd.Enqueue(session);
            };
            return Instance;
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

        /// <inheritdoc />
        public override void Shutdown()
        {
            connectionListener?.Shutdown();
        }
    }
}
