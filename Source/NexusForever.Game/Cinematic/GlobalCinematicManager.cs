using NexusForever.Game.Abstract.Cinematic;
using NexusForever.Shared;

namespace NexusForever.Game.Cinematic
{
    public sealed class GlobalCinematicManager : Singleton<GlobalCinematicManager>, IGlobalCinematicManager
    {
        /// <summary>
        /// Actors use the same unitId system as regular units (players, game play npcs) but start from a higher range
        /// </summary>
        public uint NextActorUnitId => nextActorUnitId++;

        private uint nextActorUnitId = 0x4000000;

        /// <summary>
        /// Initialises the <see cref="GlobalCinematicManager"/>.
        /// </summary>
        public void Initialise()
        {
            // Deliberately left empty
        }
    }
}
