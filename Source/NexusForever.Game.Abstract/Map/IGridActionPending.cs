using System.Numerics;

namespace NexusForever.Game.Abstract.Map
{
    public interface IGridActionPending : IGridAction
    {
        Vector3 Vector { get; init; }
    }
}