using NexusForever.Game.Abstract.Cinematic;
using NexusForever.Game.Static.Cinematic;

namespace NexusForever.Game.Abstract.Entity
{
    public interface ICinematicManager
    {
        /// <summary>
        /// Queue a <see cref="ICinematicBase"/> to be played.
        /// </summary>
        void QueueCinematic(ICinematicBase cinematic);

        /// <summary>
        /// Handle the <see cref="CinematicState"/> the Client sent back. Only to be called from Client Handlers.
        /// </summary>
        void HandleClientCinematicState(CinematicState cinematicState);
    }
}