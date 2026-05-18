using System.Numerics;

namespace NexusForever.Game.Abstract.Map
{
    public interface IGridActionAdd : IGridAction
    {
        Vector3 Vector { get; init; }
        uint RequeueCount { get; set; }
    }
}