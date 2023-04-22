using System.Collections.Immutable;

namespace NexusForever.Game.Abstract.Server
{
    public interface IServerManager
    {
        ImmutableList<IServerInfo> Servers { get; }
        ImmutableList<IServerMessageInfo> ServerMessages { get; }

        /// <summary>
        /// Initialise <see cref="IServerManager"/> and any related resources.
        /// </summary>
        void Initialise(ushort? realmId = null);

        /// <summary>
        /// Requests for the <see cref="IServerManager"/> to begin shutdown.
        /// </summary>
        void Shutdown();
    }
}