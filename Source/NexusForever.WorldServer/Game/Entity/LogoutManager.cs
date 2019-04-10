using NexusForever.Shared;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Game.Entity
{
    public class LogoutManager : IUpdate
    {
        public bool ReadyToLogout => logoutTimer <= 0d;

        public LogoutReason Reason { get; }
        public bool Requested { get; }

        private double logoutTimer;

        public LogoutManager(double timeToLogout, LogoutReason reason, bool requested)
        {
            logoutTimer = timeToLogout;
            Reason      = reason;
            Requested   = requested;
        }

        public void Update(double lastTick)
        {
            logoutTimer -= lastTick;
        }
    }
}
