using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Game.Map.Search
{
    public interface ISearchCheck
    {
        bool CheckEntity(GridEntity entity);
    }
}
