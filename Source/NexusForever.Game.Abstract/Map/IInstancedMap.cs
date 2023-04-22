using NexusForever.Game.Abstract.Entity;

namespace NexusForever.Game.Abstract.Map
{
    public interface IInstancedMap<T> where T : IMapInstance
    {
        /// <summary>
        /// Create a new instance for <see cref="IPlayer"/> with <see cref="IMapInfo"/>.
        /// </summary>
        T CreateInstance(IPlayer player, ulong? instanceId);

        /// <summary>
        /// Get an existing instance with supplied id.
        /// </summary>
        T GetInstance(ulong instanceId);
    }
}
