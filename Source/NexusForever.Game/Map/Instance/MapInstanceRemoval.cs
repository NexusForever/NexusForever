using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Abstract.Map.Instance;
using NexusForever.Game.Static.Map;

namespace NexusForever.Game.Map.Instance
{
    public class MapInstanceRemoval : IMapInstanceRemoval
    {
        public uint Guid { get; init; }
        public WorldRemovalReason Reason { get; init; }
        public IMapPosition Position { get; init; }
        public double Timer { get; set; }
    }
}
