using NexusForever.Game.Static.Map;

namespace NexusForever.Game.Abstract.Map
{
    public interface IMapInstanceRemoval
    {
        uint Guid { get; init; }
        WorldRemovalReason Reason { get; init; }
        IMapPosition Position { get; init; }
        double Timer { get; set; }
    }
}