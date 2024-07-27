using System.Numerics;
using NexusForever.Game.Abstract.Matching;

namespace NexusForever.Game.Matching
{
    public class MapEntrance : IMapEntrance
    {
        public required ushort MapId { get; init; }
        public required byte Team { get; init; }
        public required Vector3 Position { get; init; }
        public required Vector3 Rotation { get; init; }
    }
}
