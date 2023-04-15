using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.World.Message.Model;
using NexusForever.Shared.Game;
using NLog;

namespace NexusForever.Game.Entity
{
    public class LogoutManager : ILogoutManager
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Invoked when <see cref="LogoutState"/> changes to <see cref="LogoutState.Logout"/>.
        /// </summary>
        public event Action OnTimerFinished;

        public LogoutState State { get; set; }

        private UpdateTimer timer;

        private readonly IPlayer player;

        public LogoutManager(IPlayer player)
        {
            this.player = player;
        }

        public void Update(double lastTick)
        {
            timer?.Update(lastTick);
        }

        /// <summary>
        /// Start delayed logout with optional supplied time.
        /// </summary>
        public void Start(double timeToLogout = 30d)
        {
            if (timer != null)
                throw new InvalidOperationException();

            timer = new UpdateTimer(timeToLogout);
            State = LogoutState.Timer;

            player.Session.EnqueueMessageEncrypted(new ServerLogoutUpdate
            {
                TimeTillLogout     = (uint)timeToLogout * 1000,
                Unknown0           = false,
                SignatureBonusData = new ServerLogoutUpdate.SignatureBonuses
                {
                    // see FillSignatureBonuses in ExitWindow.lua for more information
                    Xp                = 0,
                    ElderPoints       = 0,
                    Currencies        = new ulong[15],
                    AccountCurrencies = new ulong[19]
                }
            });

            log.Trace($"Character {player.CharacterId} started logout.");
        }

        /// <summary>
        /// Cancel the current delayed logout.
        /// </summary>
        public void Cancel()
        {
            // can't cancel logout if not started or already complete
            if (timer == null || !timer.IsTicking)
                throw new InvalidOperationException();

            timer = null;
            State = LogoutState.None;

            log.Trace($"Character {player.CharacterId} cancelled logout.");
        }

        /// <summary>
        /// Finishes the current delayed logout with optional <see cref="LogoutReason"/>.
        /// </summary>
        /// <remarks>
        /// This can be called to start a forced logout if <see cref="LogoutReason"/> is supplied.
        /// </remarks>
        public void Finish(LogoutReason reason = LogoutReason.None)
        {
            // can't finish logout if not started or still in countdown
            if (reason == LogoutReason.None && (timer == null || timer.IsTicking))
                throw new InvalidOperationException();

            // force logout can be performed if a reason is supplied
            if (reason != LogoutReason.None)
                log.Warn($"Character {player.CharacterId} is being force logged out with reason {reason}.");

            player.Session.EnqueueMessageEncrypted(new ServerLogout
            {
                Requested = reason == LogoutReason.None,
                Reason    = reason
            });

            State = LogoutState.Logout;
            OnTimerFinished?.Invoke();
        }
    }
}
