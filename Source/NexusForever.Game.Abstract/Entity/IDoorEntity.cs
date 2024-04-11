namespace NexusForever.Game.Abstract.Entity
{
    public interface IDoorEntity : IWorldEntity
    {
        bool IsOpen { get; }

        /// <summary>
        /// Used to open this <see cref="IDoorEntity"/>.
        /// </summary>
        void OpenDoor();

        /// <summary>
        /// Used to close this <see cref="IDoorEntity"/>.
        /// </summary>
        void CloseDoor();
    }
}