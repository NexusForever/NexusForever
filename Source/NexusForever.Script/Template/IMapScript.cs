using NexusForever.Game.Abstract.Entity;
using NexusForever.Shared;

namespace NexusForever.Script.Template
{
    public interface IMapScript : IUpdate
    {
        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        void IUpdate.Update(double lastTick)
        {
        }

        /// <summary>
        /// Invoked when <see cref="IGridEntity"/> is added to map.
        /// </summary>
        void OnAddToMap(IGridEntity entity)
        {
        }

        /// <summary>
        /// Invoked when <see cref="IGridEntity"/> is removed from map.
        /// </summary>
        void OnRemoveFromMap(IGridEntity entity)
        {
        }
    }
}
