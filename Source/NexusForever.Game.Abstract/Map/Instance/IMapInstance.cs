using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map.Lock;
using NexusForever.Game.Static.Map;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Abstract.Map.Instance
{
    public interface IMapInstance : IBaseMap
    {
        /// <summary>
        /// Unique id for map instance.
        /// </summary>
        /// <remarks>
        /// Map id is unique per map, different world ids can all have instance 1, 2, 3, ect...
        /// </remarks>
        Guid InstanceId { get; }

        IMapLock MapLock { get; }

        /// <summary>
        /// Current unload status for map instance.
        /// </summary>
        /// <remarks>
        /// Status will only be set if the instance is in an unloading state.
        /// </remarks>
        MapUnloadStatus? UnloadStatus { get; }

        /// <summary>
        /// Count of players in map instance.
        /// </summary>
        uint PlayerCount { get; }

        /// <summary>
        /// Initialise <see cref="IMapInstance"/> with supplied <see cref="WorldEntry"/> and <see cref="IMapLock"/>.
        /// </summary>
        void Initialise(WorldEntry worldEntry, IMapLock mapLock);

        /// <summary>
        /// Start unloading map instance.
        /// </summary>
        /// <remarks>
        /// Any <see cref="IPlayer"/>'s still in the instance will be moved to return locations.
        /// </remarks>
        void Unload(IMapPosition unloadPosition = null);

        /// <summary>
        /// Enqueue <see cref="IPlayer"/> to be removed from map for <see cref="WorldRemovalReason"/>.
        /// After a short duration the <see cref="IPlayer"/> will be teleported their return location.
        /// </summary>
        void EnqueuePendingRemoval(IPlayer player, WorldRemovalReason reason);

        /// <summary>
        /// Cancel any pending removal for <see cref="IPlayer"/>.
        /// </summary>
        void CancelPendingRemoval(IPlayer player);

        /// <summary>
        /// Returns all <see cref="IPlayer"/>'s in map instance.
        /// </summary>
        IEnumerable<IPlayer> GetPlayers();

        /// <summary>
        /// Enqueue <see cref="IGridEntity"/> to be added to <see cref="IMapInstance"/>.
        /// </summary>
        void EnqueueAdd(IGridEntity entity, Vector3 position);
    }
}