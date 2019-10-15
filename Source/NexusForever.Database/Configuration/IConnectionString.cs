namespace NexusForever.Database.Configuration
{
    public interface IConnectionString
    {
        DatabaseProvider Provider { get; }
        string ConnectionString { get; }
    }
}
