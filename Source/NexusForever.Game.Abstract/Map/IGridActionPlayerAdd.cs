namespace NexusForever.Game.Abstract.Map
{
    public interface IGridActionPlayerAdd : IGridActionAdd
    {
        OnGenericErrorDelegate Error { get; init; }
    }
}
