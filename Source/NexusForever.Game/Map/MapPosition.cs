using System.Numerics;
using NexusForever.Game.Abstract.Map;

namespace NexusForever.Game.Map
{
    public class MapPosition : IMapPosition
    {
        public IMapInfo Info { get; init; }
        public Vector3 Position { get; set; }
    }
}
