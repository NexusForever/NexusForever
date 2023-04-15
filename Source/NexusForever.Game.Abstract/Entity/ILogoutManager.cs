using NexusForever.Game.Static.Entity;
using NexusForever.Network.World.Message.Model;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Entity
{
    public interface ILogoutManager : IUpdate
    {
        /// <summary>
        /// Invoked when <see cref="LogoutState"/> changes to <see cref="LogoutState.Logout"/>.
        /// </summary>
        event Action OnTimerFinished;

        LogoutState State { get; set; }

        /// <summary>
        /// Start delayed logout with optional supplied time.
        /// </summary>
        void Start(double timeToLogout = 30d);

        /// <summary>
        /// Cancel the current delayed logout.
        /// </summary>
        void Cancel();

        /// <summary>
        /// Finishes the current delayed logout with optional <see cref="LogoutReason"/>.
        /// </summary>
        /// <remarks>
        /// This can be called to start a forced logout if <see cref="LogoutReason"/> is supplied.
        /// </remarks>
        void Finish(LogoutReason reason = LogoutReason.None);
    }
}