using NexusForever.Game.Abstract.Entity;

namespace NexusForever.Game.Abstract.Map.Search
{
    public interface ISearchCheckFactory
    {
        TCheck CreateCheck<TEntity, TCheck>()
            where TEntity : IGridEntity
            where TCheck : ISearchCheck<TEntity>;
    }
}
