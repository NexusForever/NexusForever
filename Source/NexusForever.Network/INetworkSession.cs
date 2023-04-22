using System.Net.Sockets;
using NexusForever.Shared;
using NexusForever.Shared.Game.Events;

namespace NexusForever.Network
{
    public interface INetworkSession : IUpdate
    {
        /// <summary>
        /// Unique id for <see cref="INetworkSession"/>.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// <see cref="IEvent"/> queue.
        /// </summary>
        EventQueue Events { get; }

        /// <summary>
        /// Heartbeat to check if <see cref="INetworkSession"/> is still alive.
        /// </summary>
        SocketHeartbeat Heartbeat { get; }

        bool CanDispose();
        void ForceDisconnect();
        void OnAccept(Socket newSocket);
        void UpdateId(string id);
    }
}