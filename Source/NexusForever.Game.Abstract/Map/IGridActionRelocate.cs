using System.Numerics;

namespace NexusForever.Game.Abstract.Map
{
    public interface IGridActionRelocate : IGridAction
    {
        Vector3 Vector { get; init; }
    }
}