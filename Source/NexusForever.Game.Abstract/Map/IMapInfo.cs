using NexusForever.Game.Abstract.Map.Lock;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Abstract.Map
{
    public interface IMapInfo
    {
        WorldEntry Entry { get; init; }
        IMapLock MapLock { get; init; }
    }
}