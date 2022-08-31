using NexusForever.Shared;

namespace NexusForever.WorldServer.Game.Cinematic
{
    public class GlobalCinematicManager : Singleton<GlobalCinematicManager>
    {
        /// <summary>
        /// Id to be assigned to the next Actor
        /// </summary>
        public uint NextCinematicId => nextCinematicId++;

        private uint nextCinematicId = 1073743000;

        public GlobalCinematicManager()
        {
        }

        /// <summary>
        /// Initialises the <see cref="GlobalCinematicManager"/>
        /// </summary>
        public void Initialise()
        {
            // Deliberately left empty
        }
    }
}
