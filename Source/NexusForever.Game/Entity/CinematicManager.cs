using NexusForever.Game.Cinematic;
using NexusForever.Game.Static.Cinematic;
using NLog;

namespace NexusForever.Game.Entity
{
    public class CinematicManager
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private Player owner;

        private CinematicBase currentCinematic;
        private readonly Queue<CinematicBase> queuedCinematics = new();

        /// <summary>
        /// Initialise a <see cref="CinematicManager"/> for this <see cref="Player"/>.
        /// </summary>
        public CinematicManager(Player player)
        {
            owner = player;
        }

        /// <summary>
        /// Queue a <see cref="CinematicBase"/> to be played.
        /// </summary>
        public void QueueCinematic(CinematicBase cinematic)
        {
            queuedCinematics.Enqueue(cinematic);
            if (currentCinematic == null && queuedCinematics.Count >= 1u)
                PlayQueuedCinematic();
        }

        /// <summary>
        /// Play the next queued <see cref="CinematicBase"/>.
        /// </summary>
        public void PlayQueuedCinematic()
        {
            if (queuedCinematics.Count == 0)
                return;

            CinematicBase cinematic = queuedCinematics.Dequeue();
            currentCinematic = cinematic;
            currentCinematic.StartPlayback();
        }

        /// <summary>
        /// Handle the <see cref="CinematicState"/> the Client sent back. Only to be called from Client Handlers.
        /// </summary>
        /// <param name="cinematicState"></param>
        public void HandleClientCinematicState(CinematicState cinematicState)
        {
            switch (cinematicState)
            {
                case CinematicState.Initalising:
                    // Cast Spell: Generic - Cinematic Player State - Tier 1 (Spell4 ID: 49887)
                    // Make the Player invisible/immune from aggro
                    break;
                case CinematicState.Finishing:
                    // End Cinematic Player State spell
                    // Make the Player visible/remove immunity
                    break;
                case CinematicState.Ended:
                    // Player is back in the world. Continue any scripts that may've been paused.
                    currentCinematic = null;
                    PlayQueuedCinematic();
                    break;
            }
        }
    }
}
