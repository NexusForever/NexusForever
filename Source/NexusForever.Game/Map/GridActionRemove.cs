using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;

namespace NexusForever.Game.Map
{
    public class GridActionRemove : IGridActionRemove
    {
        public IGridEntity Entity { get; init; }
        public OnRemoveDelegate Callback { get; init; }
        public OnExceptionDelegate Exception { get; init; }
    }
}
