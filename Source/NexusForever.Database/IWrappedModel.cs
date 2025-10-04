namespace NexusForever.Database
{
    public interface IWrappedModel<T>
    {
        T Model { get; }
    }
}
