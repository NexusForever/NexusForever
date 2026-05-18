namespace NexusForever.Database.Configuration.Model
{
    public interface IConnectionString
    {
        DatabaseProvider Provider { get; }
        string ConnectionString { get; }
    }
}
