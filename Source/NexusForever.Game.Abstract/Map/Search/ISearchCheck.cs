using NexusForever.Game.Abstract.Entity;

namespace NexusForever.Game.Abstract.Map.Search
{
    public interface ISearchCheck<T> where T : IGridEntity
    {
        bool CheckEntity(T entity);
    }
}
