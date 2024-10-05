using NexusForever.Game.Abstract.Entity;

namespace NexusForever.Script.Template
{
    public interface IWorldLocationTriggerScript
    {
        /// <summary>
        /// Invoked when <see cref="IGridEntity"/> enters the trigger.
        /// </summary>
        void OnEntityEnter(IGridEntity entity)
        {
        }

        /// <summary>
        /// Invoked when <see cref="IGridEntity"/> leaves the trigger.
        /// </summary>
        void OnEntityLeave(IGridEntity entity)
        {
        }
    }
}
