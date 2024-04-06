using NexusForever.Game.Abstract.Entity;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Map
{
    public interface IMapManager : IUpdate
    {
        /// <summary>
        /// Enqueue <see cref="IPlayer"/> to be added to a map. 
        /// </summary>
        void AddToMap(IPlayer player, IMapPosition mapPosition);

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can create a new instance.
        /// </summary>
        bool CanCreateInstance(IPlayer player);

        /// <summary>
        /// Increase instance count for <see cref="IPlayer"/>.
        /// </summary>
        void IncreaseInstanceCount(IPlayer player);
    }
}