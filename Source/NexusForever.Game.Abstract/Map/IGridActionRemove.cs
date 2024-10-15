namespace NexusForever.Game.Abstract.Map
{
    public interface IGridActionRemove : IGridAction
    {
        OnRemoveDelegate Callback { get; }
    }
}