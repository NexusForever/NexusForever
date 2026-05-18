using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Abstract.Map.Lock;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Map
{
    public class MapInfo : IMapInfo
    {
        public WorldEntry Entry { get; init; }
        public IMapLock MapLock { get; init; }
    }
}
