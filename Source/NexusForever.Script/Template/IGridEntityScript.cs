using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Shared;

namespace NexusForever.Script.Template
{
    public interface IGridEntityScript : IUpdate
    {
        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        void IUpdate.Update(double lastTick)
        {
        }

        /// <summary>
        /// Invoked when <see cref="IGridEntity"/> is added to <see cref="IBaseMap"/>.
        /// </summary>
        void OnAddToMap(IBaseMap map)
        {
        }

        /// <summary>
        /// Invoked when <see cref="IGridEntity"/> is remove from <see cref="IBaseMap"/>.
        /// </summary>
        void OnRemoveFromMap(IBaseMap map)
        {
        }

        /// <summary>
        /// Invoked when <see cref="IGridEntity"/> is added to vision range.
        /// </summary>
        void OnAddVisibleEntity(IGridEntity entity)
        {
        }

        /// <summary>
        /// Invoked when <see cref="IGridEntity"/> is removed from vision range.
        /// </summary>
        void OnRemoveVisibleEntity(IGridEntity entity)
        {
        }

        /// <summary>
        /// Invoked when <see cref="IGridEntity"/> is added to range check range.
        /// </summary>
        void OnEnterRange(IGridEntity entity)
        {
        }

        /// <summary>
        /// Invoked when <see cref="IGridEntity"/> is removed from range check range.
        /// </summary>
        void OnExitRange(IGridEntity entity)
        {
        }
    }
}
