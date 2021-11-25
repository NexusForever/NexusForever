using NexusForever.Shared;
using NexusForever.Shared.Configuration;
using NexusForever.Shared.Game;
using NexusForever.Shared.Network;
using NexusForever.WorldServer.Game.RBAC;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NexusForever.WorldServer.Game
{
    public class LoginQueueManager : Singleton<LoginQueueManager>, IUpdate
    {
        private readonly Queue<WorldSession> sessionsQueued = new();

        private int connectedPlayers
        {
            get => _connectedPlayers;
            set
            {
                _connectedPlayers = Math.Clamp(value, 0, int.MaxValue);
            }
        }
        private int _connectedPlayers = 0;
        private uint maximumPlayers = ConfigurationManager<WorldServerConfiguration>.Instance.Config.MaxPlayers;
        private UpdateTimer queueCheck = new(TimeSpan.FromSeconds(5));

        private LoginQueueManager()
        {
        }

        public void Initialise()
        {
        }

        public void Update(double lastTick)
        {
            queueCheck.Update(lastTick);

            if (queueCheck.HasElapsed)
            {
                if (sessionsQueued.Count > 0u)
                {
                    while (CanEnterWorld())
                    {
                        if(sessionsQueued.TryDequeue(out WorldSession session))
                            AdmitAccount(session);
                    }

                    foreach (WorldSession remainingSession in sessionsQueued)
                        SendQueueStatus((uint)sessionsQueued.ToArray().ToList().IndexOf(remainingSession) + 1, remainingSession);
                }
                queueCheck.Reset();
            }
        }

        /// <summary>
        /// Handle a new connection to the world.
        /// </summary>
        public bool HandleNewConnection(WorldSession session)
        {
            if (session.InWorld)
                return true;

            if (!CanEnterWorld(session))
            {
                sessionsQueued.Enqueue(session);
                SendQueueStatus((uint)sessionsQueued.ToArray().ToList().IndexOf(session) + 1, session);
                return false;
            }

            connectedPlayers += 1;
            session.InWorld = true;
            return true;
        }

        /// <summary>
        /// Removes a specific session from the Queue.
        /// </summary>
        public void OnDisconnect(WorldSession session)
        {
            connectedPlayers -= 1;
            sessionsQueued.Remove(session);
        }

        /// <summary>
        /// Set the Maximum Players Allowed to be in World.
        /// </summary>
        public void SetMaxPlayers(uint newMaximumPlayers)
        {
            maximumPlayers = newMaximumPlayers;
        }

        private void AdmitAccount(WorldSession session)
        {
            connectedPlayers += 1;
            session.InWorld = true;
            session.EnqueueMessageEncrypted(new ServerQueueFinish());
        }

        private bool CanEnterWorld(WorldSession session = null)
        {
            if (session != null && session.InWorld)
                return true;

            if (session != null && session.AccountRbacManager.HasPermission(RBAC.Static.Permission.GMFlag))
                return true;

            if (connectedPlayers >= maximumPlayers)
                return false;

            return true;
        }

        private void SendQueueStatus(uint queuePosition, WorldSession session)
        {
            session.EnqueueMessageEncrypted(new ServerQueueStatus
            {
                QueuePosition = queuePosition,
                WaitTimeSeconds = (uint)(queuePosition * 30f)
            });
        }
    }
}
