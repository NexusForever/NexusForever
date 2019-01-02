namespace NexusForever.Shared.Configuration
{
    public interface IDatabaseConfiguration
    {
        IConnectionString GetConnectionString(DatabaseType type);
    }
}
