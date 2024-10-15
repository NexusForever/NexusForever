using NexusForever.Game.Abstract.Entity;

namespace NexusForever.Game.Abstract.Map.Instance
{
    public interface IInstancedMap : IMap
    {
        /// <summary>
        /// Enqueue <see cref="IPlayer"/> to be added to <see cref="IInstancedMap{T}"/>.
        /// </summary>
        /// <remarks>
        /// Characters should not be added directly through this method.
        /// Use <see cref="IPlayer.TeleportTo(IMapPosition,TeleportReason)"/> instead.
        /// </remarks>
        void EnqueueAdd(IPlayer player, IMapPosition position, OnAddDelegate callback = null, OnGenericErrorDelegate error = null, OnExceptionDelegate exception = null);
    }
}
