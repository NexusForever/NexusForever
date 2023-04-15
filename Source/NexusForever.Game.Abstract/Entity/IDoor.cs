namespace NexusForever.Game.Abstract.Entity
{
    public interface IDoor : IWorldEntity
    {
        bool IsOpen { get; }

        /// <summary>
        /// Used to open this <see cref="IDoor"/>.
        /// </summary>
        void OpenDoor();

        /// <summary>
        /// Used to close this <see cref="IDoor"/>.
        /// </summary>
        void CloseDoor();
    }
}