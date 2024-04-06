namespace NexusForever.Shared
{
    public interface IFactoryInterface<T> where T : class
    {
        T2 Resolve<T2>() where T2 : T;
    }
}
