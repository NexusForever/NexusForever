using NexusForever.Game.Abstract.Entity;

namespace NexusForever.Game.Abstract.Map.Search
{
    public interface ISearchCheck
    {
        bool CheckEntity(IGridEntity entity);
    }
}
