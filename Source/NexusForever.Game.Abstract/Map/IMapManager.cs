using NexusForever.Game.Abstract.Entity;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Map
{
    public interface IMapManager : IUpdate
    {
        /// <summary>
        /// Enqueue <see cref="IPlayer"/> to be added to a map. 
        /// </summary>
        void AddToMap(IPlayer player, IMapPosition source, IMapPosition destination, OnAddDelegate callback = null, OnGenericErrorDelegate error = null, OnExceptionDelegate exception = null);
    }
}
