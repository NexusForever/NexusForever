using NexusForever.GameTable.Model;

namespace NexusForever.Game.Abstract.Map
{
    public interface IMapInfo
    {
        WorldEntry Entry { get; init; }
        ulong? InstanceId { get; init; }
    }
}