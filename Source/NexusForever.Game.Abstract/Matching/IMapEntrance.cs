using System.Numerics;

namespace NexusForever.Game.Abstract.Matching
{
    public interface IMapEntrance
    {
        ushort MapId { get; init; }
        byte Team { get; init; }
        Vector3 Position { get; init; }
        Vector3 Rotation { get; init; }
    }
}