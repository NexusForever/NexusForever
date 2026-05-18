using NexusForever.Game.Abstract.Entity;

namespace NexusForever.Game.Abstract.Map
{
    public interface IGridAction
    {
        IGridEntity Entity { get; init; }
    }
}
