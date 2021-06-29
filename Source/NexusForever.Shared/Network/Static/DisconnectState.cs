using System.Net.Sockets;

namespace NexusForever.Shared.Network.Static
{
    public enum DisconnectState
    {
        /// <summary>
        /// Disconnection of <see cref="NetworkSession"/> has been requested.
        /// </summary>
        Pending,

        /// <summary>
        /// Disconnected of <see cref="NetworkSession"/> has been completed.
        /// Underlying <see cref="Socket"/> resources have been released.
        /// </summary>
        Complete
    }
}
