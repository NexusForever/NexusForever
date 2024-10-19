using NexusForever.Game.Abstract.Entity;

namespace NexusForever.Script.Template
{
    public interface IDoorEntityScript : IWorldEntityScript
    {
        /// <summary>
        /// Invoked when <see cref="IDoorEntity"/> is opened.
        /// </summary>
        void OnOpenDoor()
        {
        }

        /// <summary>
        /// Invoked when <see cref="IDoorEntity"/> is closed.
        /// </summary>
        void OnDoorClose()
        {
        }
    }
}
