namespace NexusForever.Game.Abstract.Cinematic
{
    public interface IGlobalCinematicManager
    {
        /// <summary>
        /// Id to be assigned to the next Cinematic.
        /// </summary>
        uint NextCinematicId { get; }

        /// <summary>
        /// Initialises the <see cref="IGlobalCinematicManager"/>.
        /// </summary>
        void Initialise();
    }
}