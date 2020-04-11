using NexusForever.WorldServer.Game.Cinematic;
using NexusForever.WorldServer.Game.Cinematic.Static;
using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.WorldServer.Game.Entity
{
    public class CinematicManager
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private Player Owner;
        private List<CinematicBase> QueuedCinematics = new List<CinematicBase>();

        /// <summary>
        /// Initialise a <see cref="CinematicManager"/> for this <see cref="Player"/>.
        /// </summary>
        public CinematicManager(Player player)
        {
            Owner = player;
        }

        /// <summary>
        /// Queue a <see cref="CinematicBase"/> to be played.
        /// </summary>
        public void QueueCinematic(CinematicBase cinematic)
        {
            QueuedCinematics.Add(cinematic);
            if (QueuedCinematics.Count == 1u)
                PlayQueuedCinematics();
        }

        /// <summary>
        /// Play the next queued <see cref="CinematicBase"/>.
        /// </summary>
        public void PlayQueuedCinematics()
        {
            if (QueuedCinematics.Count == 0)
                return;

            QueuedCinematics[0].StartPlayback();
            QueuedCinematics.RemoveAt(0);
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
                    PlayQueuedCinematics();
                    break;
            }
        }
    }
}
