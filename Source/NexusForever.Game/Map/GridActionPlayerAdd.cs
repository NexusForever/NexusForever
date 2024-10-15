using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;

namespace NexusForever.Game.Map
{
    public class GridActionPlayerAdd : IGridActionPlayerAdd
    {
        public IGridEntity Entity { get; init; }
        public Vector3 Vector { get; init; }
        public OnAddDelegate Callback { get; init; }
        public OnGenericErrorDelegate Error { get; init; }
        public OnExceptionDelegate Exception { get; init; }
        public uint RequeueCount { get; set; }
    }
}
