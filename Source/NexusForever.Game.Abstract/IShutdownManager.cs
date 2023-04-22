using NexusForever.Game.Abstract.Entity;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract
{
    public interface IShutdownManager : IUpdate
    {
        /// <summary>
        /// Returns if realm is currently pending a shutdown.
        /// </summary>
        bool IsShutdownPending { get; }

        void Initialise(Action callback);

        /// <summary>
        /// Start a realm shutdown with the supplied <see cref="TimeSpan"/> to shutdown.
        /// </summary>
        void StartShutdown(TimeSpan span);

        /// <summary>
        /// Cancel a pending realm shutdown.
        /// </summary>
        void CancelShutdown();

        /// <summary>
        /// Display pending shutdown time to <see cref="IPlayer"/> on login.
        /// </summary>
        void OnLogin(IPlayer player);
    }
}