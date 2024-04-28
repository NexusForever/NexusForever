using System;
using System.Collections.Generic;
using NexusForever.Game.Configuration.Model;
using NexusForever.Game.Static.RBAC;
using NexusForever.Network;
using NexusForever.Network.World.Message.Model;
using NexusForever.Shared;
using NexusForever.Shared.Configuration;
using NexusForever.Shared.Game;
using NexusForever.WorldServer.Network;
using NLog;

namespace NexusForever.WorldServer
{
    public sealed class LoginQueueManager : Singleton<LoginQueueManager>, ILoginQueueManager
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public class QueueData
        {
            public string Id { get; init; }
            public uint Position { get; set; }
        }

        /// <summary>
        /// Amount of sessions admitted to server.
        /// </summary>
        /// <remarks>
        /// This will not necessarily match the total amount of connected sessions or players.
        /// </remarks>
        public uint ConnectedPlayers { get; private set; }

        private readonly Dictionary<string, QueueData> queueData = new();
        private readonly Queue<string> queue = new();

        private Action<WorldSession> admitCallback;

        private uint maximumPlayers = SharedConfiguration.Instance.Get<RealmConfig>().MaxPlayers;
        private UpdateTimer queueCheck = new(TimeSpan.FromSeconds(5));

        public void Initialise(Action<WorldSession> callback)
        {
            if (admitCallback != null)
                throw new InvalidOperationException();

            admitCallback = callback;
        }

        public void Update(double lastTick)
        {
            queueCheck.Update(lastTick);
            if (!queueCheck.HasElapsed)
                return;

            if (queue.Count > 0u)
            {
                while (queue.Count > 0u && CanEnterWorld())
                {
                    if (queue.TryDequeue(out string id))
                    {
                        queueData.Remove(id);
                        AdmitSession(id);
                    }
                }

                RecalculateQueuePositions();
            }

            queueCheck.Reset();
        }

        /// <summary>
        /// Attempt to admit session to realm, if the world is full the session will be queued.
        /// </summary>
        /// <remarks>
        /// Returns <see cref="true"/> if the session was admited to the realm.
        /// </remarks>
        public bool OnNewSession(WorldSession session)
        {
            // check if session is already in queue
            // this allows the account to rejoin the queue after a disconnect
            if (queueData.TryGetValue(session.Id, out QueueData data))
            {
                log.Trace($"Session {session.Id} has rejoined the queue.");

                SendQueueStatus(session, data.Position);
                return false;
            }

            // session is not in the queue
            // check if the realm is currently accepting new sessions
            if (!CanEnterWorld(session))
            {
                log.Trace($"Session {session.Id} has joined the queue.");

                uint position = (uint)queue.Count + 1u;

                queue.Enqueue(session.Id);
                queueData.Add(session.Id, new QueueData
                {
                    Id       = session.Id,
                    Position = position
                });

                SendQueueStatus(session, position);
                return false;
            }

            AdmitSession(session);
            return true;
        }

        /// <summary>
        /// Remove session from realm queue.
        /// </summary>
        public void OnDisconnect(WorldSession session)
        {
            // current admited session count will be reduced if session isn't queued
            if (session.IsQueued != false)
                return;

            checked
            {
                ConnectedPlayers--;
            }
        }

        /// <summary>
        /// Set the maximum number of admitted sessions allowed in the realm.
        /// </summary>
        public void SetMaxPlayers(uint newMaximumPlayers)
        {
            maximumPlayers = newMaximumPlayers;
            log.Info($"Updated realm session limit to {maximumPlayers}.");
        }

        private void AdmitSession(string id)
        {
            // there is a possibility the session will not exist if the player has disconnected during the queue
            WorldSession session = NetworkManager<WorldSession>.Instance.GetSession(id);
            if (session == null)
                return;

            AdmitSession(session);

            session.EnqueueMessageEncrypted(new ServerQueueFinish());
            admitCallback.Invoke(session);
        }

        private void AdmitSession(WorldSession session)
        {
            log.Trace($"Admitting session {session.Id} into the realm.");

            session.IsQueued = false;

            checked
            {
                ConnectedPlayers++;
            }
        }

        private bool CanEnterWorld(WorldSession session)
        {
            // accounts with GM permission are exempt from queue (lucky you!)
            if (session.Account.RbacManager.HasPermission(Permission.GMFlag))
                return true;

            return CanEnterWorld();
        }

        private bool CanEnterWorld()
        {
            // potentially more checks?
            // world locked, ect...?
            return ConnectedPlayers < maximumPlayers;
        }

        private void RecalculateQueuePositions()
        {
            uint position = 1u;
            foreach (string id in queue)
            {
                if (!queueData.TryGetValue(id, out QueueData data))
                    continue;

                data.Position = position++;

                // there is a possibility the session will not exist if the player has disconnected during the queue
                WorldSession session = NetworkManager<WorldSession>.Instance.GetSession(data.Id);
                if (session != null)
                    SendQueueStatus(session, data.Position);
            }
        }

        private static void SendQueueStatus(WorldSession session, uint queuePosition)
        {
            session.EnqueueMessageEncrypted(new ServerQueueStatus
            {
                QueuePosition   = queuePosition,
                WaitTimeSeconds = (uint)(queuePosition * 30f)
            });
        }
    }
}
