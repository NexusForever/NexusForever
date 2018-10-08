using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using NexusForever.Shared.Configuration;

namespace NexusForever.Shared.Network
{
    public static class NetworkManager<T> where T : NetworkSession, new()
    {
        private static ConnectionListener<T> connectionListener;

        private static readonly ConcurrentQueue<T> pendingAdd = new ConcurrentQueue<T>();
        private static readonly ConcurrentQueue<T> pendingRemove = new ConcurrentQueue<T>();

        private static readonly HashSet<T> sessions = new HashSet<T>();

        public static void Initialise(NetworkConfig config)
        {
            connectionListener = new ConnectionListener<T>(IPAddress.Parse(config.Host), config.Port);
            connectionListener.OnNewSession += (session) =>
            {
                pendingAdd.Enqueue(session);
            };
        }

        public static T GetSession(Func<T, bool> func)
        {
            return sessions.SingleOrDefault(func);
        }

        public static void Shutdown()
        {
            connectionListener?.Shutdown();
        }

        public static void Update(double lastTick)
        {
            //
            while (pendingAdd.TryDequeue(out T session))
                sessions.Add(session);

            //
            foreach (T session in sessions)
            {
                if (session.Disconnected || session.Heartbeat.Flatline)
                    pendingRemove.Enqueue(session);
                else
                    session.Update(lastTick);
            }

            //
            while (pendingRemove.TryDequeue(out T session))
                sessions.Remove(session);
        }
    }
}
