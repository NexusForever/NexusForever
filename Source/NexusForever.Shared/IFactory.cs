namespace NexusForever.Shared
{
    public interface IFactory<T> where T : class
    {
        T Resolve();
    }
}
