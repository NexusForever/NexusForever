using NexusForever.Game.Abstract.Map;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Map
{
    public class MapInfo : IMapInfo
    {
        public WorldEntry Entry { get; init; }
        public ulong? InstanceId { get; init; }
    }
}
