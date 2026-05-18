namespace NexusForever.Game.Abstract.Map.Instance
{
    public interface IInstancedMap<T> where T : IMapInstance
    {
        /// <summary>
        /// Get an existing instance with supplied id.
        /// </summary>
        T GetInstance(Guid instanceId);
    }
}
