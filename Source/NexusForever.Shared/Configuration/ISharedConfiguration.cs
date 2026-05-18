namespace NexusForever.Shared.Configuration
{
    public interface ISharedConfiguration
    {
        void Initialise<T>();

        T Get<T>();
    }
}