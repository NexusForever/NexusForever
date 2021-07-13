using NexusForever.WorldServer.Game.Map.Static;

namespace NexusForever.WorldServer.Game.Map
{
    public class MapInstanceRemoval
    {
        public uint Guid { get; init; }
        public WorldRemovalReason Reason { get; init; }
        public MapPosition Position { get; init; }
        public double Timer { get; set; }
    }
}
