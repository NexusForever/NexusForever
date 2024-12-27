using System;
using NexusForever.Shared;
using NexusForever.WorldServer.Network;

namespace NexusForever.WorldServer
{
    public interface ILoginQueueManager : IUpdate
    {
        /// <summary>
        /// Amount of sessions admitted to server.
        /// </summary>
        /// <remarks>
        /// This will not necessarily match the total amount of connected sessions or players.
        /// </remarks>
        uint ConnectedPlayers { get; }

        void Initialise(Action<IWorldSession> callback);

        /// <summary>
        /// Attempt to admit session to realm, if the world is full the session will be queued.
        /// </summary>
        /// <remarks>
        /// Returns <see cref="true"/> if the session was admited to the realm.
        /// </remarks>
        bool OnNewSession(IWorldSession session);

        /// <summary>
        /// Remove session from realm queue.
        /// </summary>
        void OnDisconnect(IWorldSession session);

        /// <summary>
        /// Set the maximum number of admitted sessions allowed in the realm.
        /// </summary>
        void SetMaxPlayers(uint newMaximumPlayers);
    }
}