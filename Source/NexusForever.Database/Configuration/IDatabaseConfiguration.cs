namespace NexusForever.Database.Configuration
{
    public interface IDatabaseConfiguration
    {
        IConnectionString GetConnectionString(DatabaseType type);
    }
}
