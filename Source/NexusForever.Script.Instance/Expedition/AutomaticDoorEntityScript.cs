using NexusForever.Game.Abstract.Entity;
using NexusForever.Script.Template;

namespace NexusForever.Script.Instance.Expedition
{
    public abstract class AutomaticDoorEntityScript : IDoorEntityScript, IOwnedScript<IDoorEntity>
    {
        protected IDoorEntity door;

        /// <summary>
        /// Invoked when <see cref="IScript"/> is loaded.
        /// </summary>
        public void OnLoad(IDoorEntity owner)
        {
            door = owner;
            door.SetInRangeCheck(15f);
        }

        /// <summary>
        /// Invoked when <see cref="IGridEntity"/> is added to range check range.
        /// </summary>
        public virtual void OnEnterRange(IGridEntity entity)
        {
            if (entity is not IPlayer)
                return;

            if (door.IsOpen)
                return;

            door.OpenDoor();
        }

        /// <summary>
        /// Invoked when <see cref="IDoorEntity"/> is opened.
        /// </summary>
        public virtual void OnOpenDoor()
        {
            // deliberately empty
        }

        /// <summary>
        /// Invoked when <see cref="IDoorEntity"/> is closed.
        /// </summary>
        public virtual void OnDoorClose()
        {
            // deliberately empty
        }
    }
}
