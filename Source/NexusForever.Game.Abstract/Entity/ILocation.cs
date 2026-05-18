using System.Numerics;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Abstract.Entity
{
    public interface ILocation
    {
        WorldEntry World { get; }
        Vector3 Position { get; }
        Vector3 Rotation { get; }
    }
}