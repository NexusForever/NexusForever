using System.Numerics;

namespace NexusForever.Game.Abstract.Map
{
    public interface IMapPosition
    {
        IMapInfo Info { get; init; }
        Vector3 Position { get; init; }
    }
}