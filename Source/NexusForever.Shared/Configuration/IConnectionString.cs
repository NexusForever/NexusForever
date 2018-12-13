namespace NexusForever.Shared.Configuration
{
    public interface IConnectionString
    {
        DatabaseProvider Provider { get; }
        string ConnectionString { get; }
    }
}
